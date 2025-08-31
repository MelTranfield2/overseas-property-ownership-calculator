using LandValueTaxCalculator.Data;
using LandValueTaxCalculator.Models;
using LandValueTaxCalculator.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LandValueTaxCalculator.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly PropertyContext _context;
        private readonly ITaxCalculationService _taxCalculationService;

        public PropertyService(PropertyContext context, ITaxCalculationService taxCalculationService)
        {
            _context = context;
            _taxCalculationService = taxCalculationService;
        }

        public async Task<List<Property>> GetAllPropertiesAsync()
        {
            return await _context.Properties.ToListAsync();
        }

        public async Task<List<Property>> GetPropertiesByRegionAsync(string region)
        {
            return await _context.Properties
                .Where(p => p.Region == region)
                .OrderBy(p => p.Address)
                .ToListAsync();
        }

        public async Task<List<string>> GetAvailableRegionsAsync()
        {
            return await _context.Properties
                .Select(p => p.Region)
                .Distinct()
                .OrderBy(r => r)
                .ToListAsync();
        }

        public async Task<RegionSummary> GetRegionSummaryAsync(string region)
        {
            var properties = await GetPropertiesByRegionAsync(region);
            var taxCalculations = _taxCalculationService.CalculateTaxForProperties(properties);

            return new RegionSummary
            {
                Region = region,
                Properties = properties,
                TaxCalculations = taxCalculations,
                PropertyCount = properties.Count,
                TotalValue = properties.Sum(p => p.EstimatedValue),
                TotalPotentialTax = taxCalculations.Sum(tc => tc.AnnualTax),
                CenterLatitude = properties.Any() ? properties.Average(p => p.Latitude) : 51.5074,
                CenterLongitude = properties.Any() ? properties.Average(p => p.Longitude) : -0.1278
            };
        }
    }
}