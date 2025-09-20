namespace DomesticRenewablesCalculator.Models;

public record SolarSystem(
    bool IsEnabled,
    double SystemSizeKw,
    double GenerationPerKwPerDay,
    double InstallCost,
    double MaintenancePerYear,
    double LifetimeYears);
