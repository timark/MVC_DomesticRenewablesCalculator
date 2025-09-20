using DomesticRenewablesCalculator.Models;

namespace DomesticRenewablesCalculator.Services;

public class RoiCalculatorService : IRoiCalculatorService
{
    public InvestmentResult Calculate(
        TariffProfile tariff,
        UsagePattern usagePattern,
        SolarSystem solar,
        BatterySystem battery,
        HotWaterStorageSystem hotWater,
        InvestmentAnalysisOptions options)
    {
        ArgumentNullException.ThrowIfNull(tariff);
        ArgumentNullException.ThrowIfNull(usagePattern);
        ArgumentNullException.ThrowIfNull(solar);
        ArgumentNullException.ThrowIfNull(battery);
        ArgumentNullException.ThrowIfNull(hotWater);
        ArgumentNullException.ThrowIfNull(options);

        var annualConsumption = Math.Max(0d, usagePattern.AverageDailyConsumptionKWh) * 365d;

        var peakFraction = usagePattern.PeakUsageFraction;
        var shoulderFraction = usagePattern.ShoulderUsageFraction;
        var offPeakFraction = usagePattern.OffPeakUsageFraction;
        var fractionSum = peakFraction + shoulderFraction + offPeakFraction;
        if (fractionSum <= 0)
        {
            peakFraction = shoulderFraction = offPeakFraction = 1d / 3d;
        }
        else
        {
            peakFraction = peakFraction / fractionSum;
            shoulderFraction = shoulderFraction / fractionSum;
            offPeakFraction = offPeakFraction / fractionSum;
        }

        var baselinePeakConsumption = annualConsumption * peakFraction;
        var baselineShoulderConsumption = annualConsumption * shoulderFraction;
        var baselineOffPeakConsumption = annualConsumption * offPeakFraction;

        var baselineAnnualCost = baselinePeakConsumption * tariff.PeakRate
                                 + baselineShoulderConsumption * tariff.ShoulderRate
                                 + baselineOffPeakConsumption * tariff.OffPeakRate
                                 + tariff.DailySupplyCharge * 365d;

        var gridPeakConsumption = baselinePeakConsumption;
        var gridShoulderConsumption = baselineShoulderConsumption;
        var gridOffPeakConsumption = baselineOffPeakConsumption;

        var solarGeneration = 0d;
        var solarSelfConsumption = 0d;
        var solarExport = 0d;
        var batterySolarCharge = 0d;
        var batteryOffPeakCharge = 0d;
        var batteryDischarge = 0d;
        var batteryContributionPeak = 0d;
        var batteryContributionShoulder = 0d;
        var hotWaterShiftedLoad = 0d;
        var hotWaterAdditionalOffPeak = 0d;

        if (solar.IsEnabled && solar.SystemSizeKw > 0 && solar.GenerationPerKwPerDay > 0)
        {
            solarGeneration = solar.SystemSizeKw * solar.GenerationPerKwPerDay * 365d;
            var daytimeFraction = Math.Clamp(usagePattern.DaytimeUsageFraction, 0d, 1d);
            var daytimeConsumption = annualConsumption * daytimeFraction;

            solarSelfConsumption = Math.Min(daytimeConsumption, solarGeneration);

            var solarToShoulder = Math.Min(gridShoulderConsumption, solarSelfConsumption);
            gridShoulderConsumption -= solarToShoulder;
            var remainingSolarSelf = solarSelfConsumption - solarToShoulder;
            var solarToPeak = Math.Min(gridPeakConsumption, remainingSolarSelf);
            gridPeakConsumption -= solarToPeak;

            solarExport = Math.Max(0d, solarGeneration - solarSelfConsumption);
        }

        var solarSurplusAvailable = solarExport;

        if (battery.IsEnabled
            && battery.CapacityKWh > 0
            && battery.RoundTripEfficiency > 0
            && battery.DepthOfDischarge > 0)
        {
            var usableCapacity = Math.Max(0d, battery.CapacityKWh * Math.Clamp(battery.DepthOfDischarge, 0d, 1d));
            var roundTripEfficiency = Math.Clamp(battery.RoundTripEfficiency, 0d, 1d);
            var offPeakChargingFraction = Math.Clamp(battery.ChargeFromOffPeakFraction, 0d, 1d);

            if (usableCapacity > 0 && roundTripEfficiency > 0)
            {
                var annualBatteryCapacity = usableCapacity * 365d;
                var targetSolarCharge = annualBatteryCapacity * (1d - offPeakChargingFraction);
                batterySolarCharge = Math.Min(solarSurplusAvailable, targetSolarCharge);
                solarSurplusAvailable -= batterySolarCharge;

                var remainingCapacity = annualBatteryCapacity - batterySolarCharge;
                var maximumOffPeakCharge = annualBatteryCapacity * offPeakChargingFraction;
                batteryOffPeakCharge = Math.Min(remainingCapacity, maximumOffPeakCharge);
                gridOffPeakConsumption += batteryOffPeakCharge;

                var totalCharge = batterySolarCharge + batteryOffPeakCharge;
                batteryDischarge = totalCharge * roundTripEfficiency;

                batteryContributionPeak = Math.Min(gridPeakConsumption, batteryDischarge);
                gridPeakConsumption -= batteryContributionPeak;

                var remainingDischarge = batteryDischarge - batteryContributionPeak;
                batteryContributionShoulder = Math.Min(gridShoulderConsumption, remainingDischarge);
                gridShoulderConsumption -= batteryContributionShoulder;
            }
        }

        solarExport = solarSurplusAvailable;

        if (hotWater.IsEnabled && hotWater.ShiftableLoadKWhPerDay > 0)
        {
            var annualShiftableLoad = hotWater.ShiftableLoadKWhPerDay * 365d;
            var storageEfficiency = Math.Clamp(hotWater.StorageEfficiency, 0.01d, 1d);

            var peakReduction = Math.Min(gridPeakConsumption, annualShiftableLoad);
            gridPeakConsumption -= peakReduction;
            var remainingShiftLoad = annualShiftableLoad - peakReduction;
            var shoulderReduction = Math.Min(gridShoulderConsumption, remainingShiftLoad);
            gridShoulderConsumption -= shoulderReduction;

            hotWaterShiftedLoad = peakReduction + shoulderReduction;
            if (hotWaterShiftedLoad > 0)
            {
                hotWaterAdditionalOffPeak = hotWaterShiftedLoad / storageEfficiency;
                gridOffPeakConsumption += hotWaterAdditionalOffPeak;
            }
        }

        gridPeakConsumption = Math.Max(0d, gridPeakConsumption);
        gridShoulderConsumption = Math.Max(0d, gridShoulderConsumption);
        gridOffPeakConsumption = Math.Max(0d, gridOffPeakConsumption);

        solarExport = Math.Max(0d, solarExport);
        solarSelfConsumption = Math.Max(0d, solarSelfConsumption);

        var annualFeedInRevenue = solarExport * tariff.FeedInTariff;

        var annualGridCost = gridPeakConsumption * tariff.PeakRate
                             + gridShoulderConsumption * tariff.ShoulderRate
                             + gridOffPeakConsumption * tariff.OffPeakRate
                             + tariff.DailySupplyCharge * 365d;

        var annualMaintenanceCost = 0d;
        if (solar.IsEnabled)
        {
            annualMaintenanceCost += Math.Max(0d, solar.MaintenancePerYear);
        }

        if (battery.IsEnabled)
        {
            annualMaintenanceCost += Math.Max(0d, battery.MaintenancePerYear);
        }

        if (hotWater.IsEnabled)
        {
            annualMaintenanceCost += Math.Max(0d, hotWater.MaintenancePerYear);
        }

        var annualOperatingCost = annualGridCost - annualFeedInRevenue + annualMaintenanceCost;
        var annualNetSavings = baselineAnnualCost - annualOperatingCost;

        var totalUpfrontCost = 0d;
        if (solar.IsEnabled)
        {
            totalUpfrontCost += Math.Max(0d, solar.InstallCost);
        }

        if (battery.IsEnabled)
        {
            totalUpfrontCost += Math.Max(0d, battery.InstallCost);
        }

        if (hotWater.IsEnabled)
        {
            totalUpfrontCost += Math.Max(0d, hotWater.InstallCost);
        }

        double? simplePaybackYears = null;
        if (totalUpfrontCost > 0 && annualNetSavings > 0)
        {
            simplePaybackYears = totalUpfrontCost / annualNetSavings;
        }

        var analysisYears = Math.Max(1, options.AnalysisYears);
        var discountRate = Math.Max(0d, options.DiscountRate);

        var netPresentValue = -totalUpfrontCost;
        for (var year = 1; year <= analysisYears; year++)
        {
            netPresentValue += annualNetSavings / Math.Pow(1 + discountRate, year);
        }

        var totalNetSavingsOverPeriod = annualNetSavings * analysisYears - totalUpfrontCost;
        var simpleReturnOnInvestment = totalUpfrontCost > 0
            ? totalNetSavingsOverPeriod / totalUpfrontCost
            : 0d;

        return new InvestmentResult
        {
            BaselineAnnualConsumptionKWh = annualConsumption,
            BaselinePeakConsumptionKWh = baselinePeakConsumption,
            BaselineShoulderConsumptionKWh = baselineShoulderConsumption,
            BaselineOffPeakConsumptionKWh = baselineOffPeakConsumption,
            BaselineAnnualCost = baselineAnnualCost,
            SolarGenerationKWh = solarGeneration,
            SolarSelfConsumptionKWh = solarSelfConsumption,
            SolarExportKWh = solarExport,
            BatterySolarChargeKWh = batterySolarCharge,
            BatteryOffPeakChargeKWh = batteryOffPeakCharge,
            BatteryDischargeKWh = batteryDischarge,
            BatteryContributionToPeakKWh = batteryContributionPeak,
            BatteryContributionToShoulderKWh = batteryContributionShoulder,
            HotWaterShiftedLoadKWh = hotWaterShiftedLoad,
            HotWaterAdditionalOffPeakKWh = hotWaterAdditionalOffPeak,
            GridPeakConsumptionWithSystemKWh = gridPeakConsumption,
            GridShoulderConsumptionWithSystemKWh = gridShoulderConsumption,
            GridOffPeakConsumptionWithSystemKWh = gridOffPeakConsumption,
            AnnualFeedInRevenue = annualFeedInRevenue,
            AnnualGridCostWithSystem = annualGridCost,
            AnnualMaintenanceCost = annualMaintenanceCost,
            AnnualOperatingCostWithSystem = annualOperatingCost,
            AnnualNetSavings = annualNetSavings,
            TotalUpfrontCost = totalUpfrontCost,
            SimplePaybackYears = simplePaybackYears,
            NetPresentValue = netPresentValue,
            TotalNetSavingsOverPeriod = totalNetSavingsOverPeriod,
            SimpleReturnOnInvestment = simpleReturnOnInvestment
        };
    }
}
