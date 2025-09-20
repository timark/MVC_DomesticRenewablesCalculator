namespace DomesticRenewablesCalculator.Models;

public record BatterySystem(
    bool IsEnabled,
    double CapacityKWh,
    double RoundTripEfficiency,
    double DepthOfDischarge,
    double InstallCost,
    double MaintenancePerYear,
    double LifetimeYears,
    double ChargeFromOffPeakFraction);
