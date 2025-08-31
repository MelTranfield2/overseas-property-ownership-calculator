using LandValueTaxCalculator.Models;

namespace LandValueTaxCalculator.Services
{
    public class TaxCalculationService : ITaxCalculationService
    {
        public TaxCalculation CalculateTax(Property property, decimal taxRate = 0.01m)
        {
            
            var landValuePortion = property.EstimatedValue * 0.7m;
            var annualTax = landValuePortion * taxRate;

            return new TaxCalculation
            {
                Property = property,
                TaxRate = taxRate,
                LandValuePortion = landValuePortion,
                AnnualTax = annualTax
            };
        }

        public List<TaxCalculation> CalculateTaxForProperties(List<Property> properties, decimal taxRate = 0.01m)
        {
            return properties.Select(p => CalculateTax(p, taxRate)).ToList();
        }
    }
}