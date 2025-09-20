using DomesticRenewablesCalculator.Models;
using DomesticRenewablesCalculator.Services;
using DomesticRenewablesCalculator.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DomesticRenewablesCalculator.Controllers;

public class HomeController : Controller
{
    private readonly IRoiCalculatorService _roiCalculatorService;

    public HomeController(IRoiCalculatorService roiCalculatorService)
    {
        _roiCalculatorService = roiCalculatorService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var viewModel = new InvestmentInputViewModel();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(InvestmentInputViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var usageTotal = viewModel.UsagePattern.PeakUsagePercentage
                         + viewModel.UsagePattern.ShoulderUsagePercentage
                         + viewModel.UsagePattern.OffPeakUsagePercentage;
        if (Math.Abs(usageTotal - 100d) > 0.01d)
        {
            ViewData["UsageNormalisationMessage"] =
                "Usage percentages were normalised to 100% during the calculation.";
        }

        var tariffProfile = new TariffProfile(
            viewModel.Tariff.PeakRate,
            viewModel.Tariff.ShoulderRate,
            viewModel.Tariff.OffPeakRate,
            viewModel.Tariff.FeedInTariff,
            viewModel.Tariff.DailySupplyCharge);

        var usagePattern = new UsagePattern(
            viewModel.UsagePattern.AverageDailyConsumptionKWh,
            viewModel.UsagePattern.PeakUsagePercentage / 100d,
            viewModel.UsagePattern.ShoulderUsagePercentage / 100d,
            viewModel.UsagePattern.OffPeakUsagePercentage / 100d,
            viewModel.UsagePattern.DaytimeUsagePercentage / 100d);

        var solarSystem = new SolarSystem(
            viewModel.Solar.IsEnabled,
            viewModel.Solar.SystemSizeKw,
            viewModel.Solar.GenerationPerKwPerDay,
            viewModel.Solar.InstallCost,
            viewModel.Solar.MaintenancePerYear,
            viewModel.Solar.LifetimeYears);

        var batterySystem = new BatterySystem(
            viewModel.Battery.IsEnabled,
            viewModel.Battery.CapacityKWh,
            viewModel.Battery.RoundTripEfficiencyPercentage / 100d,
            viewModel.Battery.DepthOfDischargePercentage / 100d,
            viewModel.Battery.InstallCost,
            viewModel.Battery.MaintenancePerYear,
            viewModel.Battery.LifetimeYears,
            viewModel.Battery.ChargeFromOffPeakPercentage / 100d);

        var hotWaterSystem = new HotWaterStorageSystem(
            viewModel.HotWater.IsEnabled,
            viewModel.HotWater.ShiftableLoadKWhPerDay,
            viewModel.HotWater.StorageEfficiencyPercentage / 100d,
            viewModel.HotWater.InstallCost,
            viewModel.HotWater.MaintenancePerYear,
            viewModel.HotWater.LifetimeYears);

        var analysisOptions = new InvestmentAnalysisOptions(
            viewModel.AnalysisOptions.AnalysisYears,
            viewModel.AnalysisOptions.DiscountRatePercentage / 100d);

        var result = _roiCalculatorService.Calculate(
            tariffProfile,
            usagePattern,
            solarSystem,
            batterySystem,
            hotWaterSystem,
            analysisOptions);

        viewModel.Result = result;

        return View(viewModel);
    }
}
