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

        private double GetLatitudeFromPostcode(string postcode)
        {
            // Simple postcode to lat/lng mapping for UK
            var area = postcode.Length >= 2 ? postcode.Substring(0, 2).ToUpper() : "XX";
            return area switch
            {
                "E1" or "EC" or "WC" or "SW" or "SE" or "NW" or "N1" => 51.5074, // London
                "M1" or "M2" or "M3" => 53.4808, // Manchester
                "B1" or "B2" => 52.4862, // Birmingham
                "LS" => 53.8008, // Leeds
                "S1" => 53.3811, // Sheffield
                _ => 52.3555 // Default UK center
            };
        }

        private double GetLongitudeFromPostcode(string postcode)
        {
            var area = postcode.Length >= 2 ? postcode.Substring(0, 2).ToUpper() : "XX";
            return area switch
            {
                "E1" or "EC" or "WC" or "SW" or "SE" or "NW" or "N1" => -0.1278, // London
                "M1" or "M2" or "M3" => -2.2426, // Manchester
                "B1" or "B2" => -1.8904, // Birmingham
                "LS" => -1.5491, // Leeds
                "S1" => -1.4701, // Sheffield
                _ => -1.1743 // Default UK center
            };
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