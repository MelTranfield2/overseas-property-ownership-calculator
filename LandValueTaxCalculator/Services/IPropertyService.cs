using LandValueTaxCalculator.Models;
using LandValueTaxCalculator.Models.ViewModels;

namespace LandValueTaxCalculator.Services
{
    public interface IPropertyService
    {
        Task<List<Property>> GetAllPropertiesAsync();
        Task<List<Property>> GetPropertiesByRegionAsync(string region);
        Task<List<string>> GetAvailableRegionsAsync();
        Task<RegionSummary> GetRegionSummaryAsync(string region);
    }
}