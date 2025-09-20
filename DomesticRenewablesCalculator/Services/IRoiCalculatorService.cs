using DomesticRenewablesCalculator.Models;

namespace DomesticRenewablesCalculator.Services;

public interface IRoiCalculatorService
{
    InvestmentResult Calculate(
        TariffProfile tariff,
        UsagePattern usagePattern,
        SolarSystem solar,
        BatterySystem battery,
        HotWaterStorageSystem hotWater,
        InvestmentAnalysisOptions options);
}
