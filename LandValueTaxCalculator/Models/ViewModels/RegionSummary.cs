namespace LandValueTaxCalculator.Models.ViewModels;

public class RegionSummary
{
    public string Region { get; set; } = string.Empty;
    public int PropertyCount { get; set; }
    public decimal TotalValue { get; set; }
    public decimal TotalPotentialTax { get; set; }
    public List<Property> Properties { get; set; } = new();
    public List<TaxCalculation> TaxCalculations { get; set; } = new();
        
    // For map display
    public double CenterLatitude { get; set; }
    public double CenterLongitude { get; set; }
}