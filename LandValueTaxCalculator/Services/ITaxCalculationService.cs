using LandValueTaxCalculator.Models;

namespace LandValueTaxCalculator.Services;

public interface ITaxCalculationService
{
    TaxCalculation CalculateTax(Property property, decimal taxRate = 0.01m);
    List<TaxCalculation> CalculateTaxForProperties(List<Property> properties, decimal taxRate = 0.01m);
}