namespace LandValueTaxCalculator.Models.ViewModels;

public class SearchViewModel
{
    public string? SelectedRegion { get; set; }
    public List<string> AvailableRegions { get; set; } = new();
    public RegionSummary? Summary { get; set; }
}