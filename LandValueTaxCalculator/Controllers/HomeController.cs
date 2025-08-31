using Microsoft.AspNetCore.Mvc;
using LandValueTaxCalculator.Models;
using LandValueTaxCalculator.Models.ViewModels;
using LandValueTaxCalculator.Services;
using System.Diagnostics;

namespace LandValueTaxCalculator.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPropertyService _propertyService;

        public HomeController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new SearchViewModel
            {
                AvailableRegions = await _propertyService.GetAvailableRegionsAsync()
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string selectedRegion)
        {
            var viewModel = new SearchViewModel
            {
                SelectedRegion = selectedRegion,
                AvailableRegions = await _propertyService.GetAvailableRegionsAsync()
            };

            if (!string.IsNullOrEmpty(selectedRegion))
            {
                var regionSummary = await _propertyService.GetRegionSummaryAsync(selectedRegion);
                viewModel.Summary = regionSummary;
            }

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}