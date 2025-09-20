namespace DomesticRenewablesCalculator.Models;

public record InvestmentResult
{
    public double BaselineAnnualConsumptionKWh { get; init; }
    public double BaselinePeakConsumptionKWh { get; init; }
    public double BaselineShoulderConsumptionKWh { get; init; }
    public double BaselineOffPeakConsumptionKWh { get; init; }
    public double BaselineAnnualCost { get; init; }
    public double SolarGenerationKWh { get; init; }
    public double SolarSelfConsumptionKWh { get; init; }
    public double SolarExportKWh { get; init; }
    public double BatterySolarChargeKWh { get; init; }
    public double BatteryOffPeakChargeKWh { get; init; }
    public double BatteryDischargeKWh { get; init; }
    public double BatteryContributionToPeakKWh { get; init; }
    public double BatteryContributionToShoulderKWh { get; init; }
    public double HotWaterShiftedLoadKWh { get; init; }
    public double HotWaterAdditionalOffPeakKWh { get; init; }
    public double GridPeakConsumptionWithSystemKWh { get; init; }
    public double GridShoulderConsumptionWithSystemKWh { get; init; }
    public double GridOffPeakConsumptionWithSystemKWh { get; init; }
    public double AnnualFeedInRevenue { get; init; }
    public double AnnualGridCostWithSystem { get; init; }
    public double AnnualMaintenanceCost { get; init; }
    public double AnnualOperatingCostWithSystem { get; init; }
    public double AnnualNetSavings { get; init; }
    public double TotalUpfrontCost { get; init; }
    public double? SimplePaybackYears { get; init; }
    public double NetPresentValue { get; init; }
    public double TotalNetSavingsOverPeriod { get; init; }
    public double SimpleReturnOnInvestment { get; init; }
}
