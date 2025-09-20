namespace DomesticRenewablesCalculator.Models;

public record UsagePattern(
    double AverageDailyConsumptionKWh,
    double PeakUsageFraction,
    double ShoulderUsageFraction,
    double OffPeakUsageFraction,
    double DaytimeUsageFraction);
