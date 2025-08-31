namespace LandValueTaxCalculator.Models;

public class Property
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;
    public decimal EstimatedValue { get; set; }
    public string OwnerCompany { get; set; } = string.Empty;
    public string OwnerCountry { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTimeOffset PurchaseDate { get; set; }
    public bool IsUKCompany { get; set; } = false; // New property to distinguish UK companies
    public string CompanyType { get; set; } = string.Empty; // For UK companies
}