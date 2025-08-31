namespace LandValueTaxCalculator.Models;

public class TaxCalculation
{
    public Property Property { get; set; } = new();
    public decimal TaxRate { get; set; }
    public decimal AnnualTax { get; set; }
    public decimal LandValuePortion { get; set; }
}