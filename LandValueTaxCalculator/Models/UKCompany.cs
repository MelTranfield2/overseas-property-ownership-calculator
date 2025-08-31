namespace LandValueTaxCalculator.Models
{
    public class UKCompany
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CompanyType { get; set; } = string.Empty;
        public string RegisteredAddress { get; set; } = string.Empty;
        public string PostCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}