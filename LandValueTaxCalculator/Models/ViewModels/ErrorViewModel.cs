using LandValueTaxCalculator.Models.ViewModels;

namespace LandValueTaxCalculator.Models.ViewModels
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}