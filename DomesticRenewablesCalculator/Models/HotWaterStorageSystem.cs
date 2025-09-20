namespace DomesticRenewablesCalculator.Models;

public record HotWaterStorageSystem(
    bool IsEnabled,
    double ShiftableLoadKWhPerDay,
    double StorageEfficiency,
    double InstallCost,
    double MaintenancePerYear,
    double LifetimeYears);
