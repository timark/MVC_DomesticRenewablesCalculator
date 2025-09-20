namespace DomesticRenewablesCalculator.Models;

public record TariffProfile(
    double PeakRate,
    double ShoulderRate,
    double OffPeakRate,
    double FeedInTariff,
    double DailySupplyCharge);
