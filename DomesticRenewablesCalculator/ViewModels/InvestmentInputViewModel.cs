using System.ComponentModel.DataAnnotations;
using DomesticRenewablesCalculator.Models;

namespace DomesticRenewablesCalculator.ViewModels;

public class InvestmentInputViewModel
{
    public TariffInputModel Tariff { get; set; } = new();

    public UsagePatternInputModel UsagePattern { get; set; } = new();

    public SolarInputModel Solar { get; set; } = new();

    public BatteryInputModel Battery { get; set; } = new();

    public HotWaterInputModel HotWater { get; set; } = new();

    public AnalysisOptionsInputModel AnalysisOptions { get; set; } = new();

    public InvestmentResult? Result { get; set; }
}

public class TariffInputModel
{
    [Display(Name = "Peak rate ($/kWh)")]
    [Range(0, double.MaxValue)]
    public double PeakRate { get; set; } = 0.32d;

    [Display(Name = "Shoulder rate ($/kWh)")]
    [Range(0, double.MaxValue)]
    public double ShoulderRate { get; set; } = 0.24d;

    [Display(Name = "Off-peak rate ($/kWh)")]
    [Range(0, double.MaxValue)]
    public double OffPeakRate { get; set; } = 0.18d;

    [Display(Name = "Feed-in tariff ($/kWh)")]
    [Range(0, double.MaxValue)]
    public double FeedInTariff { get; set; } = 0.08d;

    [Display(Name = "Daily supply charge ($)")]
    [Range(0, double.MaxValue)]
    public double DailySupplyCharge { get; set; } = 0.9d;
}

public class UsagePatternInputModel
{
    [Display(Name = "Average daily consumption (kWh)")]
    [Range(0, double.MaxValue)]
    public double AverageDailyConsumptionKWh { get; set; } = 20d;

    [Display(Name = "Peak usage (%)")]
    [Range(0, 100)]
    public double PeakUsagePercentage { get; set; } = 40d;

    [Display(Name = "Shoulder usage (%)")]
    [Range(0, 100)]
    public double ShoulderUsagePercentage { get; set; } = 35d;

    [Display(Name = "Off-peak usage (%)")]
    [Range(0, 100)]
    public double OffPeakUsagePercentage { get; set; } = 25d;

    [Display(Name = "Daytime usage during solar generation (%)")]
    [Range(0, 100)]
    public double DaytimeUsagePercentage { get; set; } = 30d;
}

public class SolarInputModel
{
    [Display(Name = "Include solar PV")]
    public bool IsEnabled { get; set; } = true;

    [Display(Name = "System size (kW)")]
    [Range(0, 100)]
    public double SystemSizeKw { get; set; } = 6.6d;

    [Display(Name = "Average generation per kW per day (kWh)")]
    [Range(0, 24)]
    public double GenerationPerKwPerDay { get; set; } = 4.2d;

    [Display(Name = "Upfront cost ($)")]
    [Range(0, double.MaxValue)]
    public double InstallCost { get; set; } = 6500d;

    [Display(Name = "Annual maintenance cost ($)")]
    [Range(0, double.MaxValue)]
    public double MaintenancePerYear { get; set; } = 120d;

    [Display(Name = "Expected lifetime (years)")]
    [Range(1, 50)]
    public double LifetimeYears { get; set; } = 20d;
}

public class BatteryInputModel
{
    [Display(Name = "Include battery storage")]
    public bool IsEnabled { get; set; } = true;

    [Display(Name = "Usable capacity (kWh)")]
    [Range(0, 200)]
    public double CapacityKWh { get; set; } = 10d;

    [Display(Name = "Round-trip efficiency (%)")]
    [Range(0, 100)]
    public double RoundTripEfficiencyPercentage { get; set; } = 90d;

    [Display(Name = "Depth of discharge (%)")]
    [Range(0, 100)]
    public double DepthOfDischargePercentage { get; set; } = 90d;

    [Display(Name = "Portion charged from off-peak grid energy (%)")]
    [Range(0, 100)]
    public double ChargeFromOffPeakPercentage { get; set; } = 20d;

    [Display(Name = "Upfront cost ($)")]
    [Range(0, double.MaxValue)]
    public double InstallCost { get; set; } = 11000d;

    [Display(Name = "Annual maintenance cost ($)")]
    [Range(0, double.MaxValue)]
    public double MaintenancePerYear { get; set; } = 150d;

    [Display(Name = "Expected lifetime (years)")]
    [Range(1, 25)]
    public double LifetimeYears { get; set; } = 12d;
}

public class HotWaterInputModel
{
    [Display(Name = "Include hot water storage")]
    public bool IsEnabled { get; set; } = false;

    [Display(Name = "Shiftable hot water load per day (kWh)")]
    [Range(0, 50)]
    public double ShiftableLoadKWhPerDay { get; set; } = 6d;

    [Display(Name = "Storage efficiency (%)")]
    [Range(1, 100)]
    public double StorageEfficiencyPercentage { get; set; } = 85d;

    [Display(Name = "Upfront cost ($)")]
    [Range(0, double.MaxValue)]
    public double InstallCost { get; set; } = 4000d;

    [Display(Name = "Annual maintenance cost ($)")]
    [Range(0, double.MaxValue)]
    public double MaintenancePerYear { get; set; } = 80d;

    [Display(Name = "Expected lifetime (years)")]
    [Range(1, 25)]
    public double LifetimeYears { get; set; } = 15d;
}

public class AnalysisOptionsInputModel
{
    [Display(Name = "Analysis period (years)")]
    [Range(1, 40)]
    public int AnalysisYears { get; set; } = 20;

    [Display(Name = "Discount rate (%)")]
    [Range(0, 100)]
    public double DiscountRatePercentage { get; set; } = 5d;
}
