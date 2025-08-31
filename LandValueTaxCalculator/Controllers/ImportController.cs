using Microsoft.AspNetCore.Mvc;
using LandValueTaxCalculator.Services;

namespace LandValueTaxCalculator.Controllers
{
    public class ImportController : Controller
    {
        private readonly DataImportService _importService;

        public ImportController(DataImportService importService)
        {
            _importService = importService;
        }

        [HttpPost]
        public async Task<IActionResult> ImportUKCompanies()
        {
            try
            {
                await _importService.ImportUKCompaniesAsync("uk_companies.csv");
                return Json(new { success = true, message = "Import completed successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}