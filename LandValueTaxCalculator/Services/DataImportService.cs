using LandValueTaxCalculator.Data;
using LandValueTaxCalculator.Models;
using Microsoft.EntityFrameworkCore;

namespace LandValueTaxCalculator.Services
{
    public class DataImportService
    {
        private readonly PropertyContext _context;

        public DataImportService(PropertyContext context)
        {
            _context = context;
        }

        public async Task ImportUKCompaniesAsync(string csvPath)
        {
            var existingCount = await _context.UKCompanies.CountAsync();
            if (existingCount > 0)
            {
                Console.WriteLine($"Database already has {existingCount:N0} companies");
                return;
            }

            Console.WriteLine($"Starting import from: {csvPath}");
            const int batchSize = 10000;
            var companies = new List<UKCompany>();
            var totalImported = 0;
            var startTime = DateTime.Now;

            var lines = await File.ReadAllLinesAsync(csvPath);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var company = ParseCompanyFromCsv(line);
                if (company != null)
                {
                    companies.Add(company);
                }

                if (companies.Count >= batchSize)
                {
                    await _context.UKCompanies.AddRangeAsync(companies);
                    await _context.SaveChangesAsync();
                    
                    totalImported += companies.Count;
                    var rate = totalImported / (DateTime.Now - startTime).TotalMinutes;
                    Console.WriteLine($"Imported {totalImported:N0} ({rate:F0}/min)");
                    
                    companies.Clear();
                    _context.ChangeTracker.Clear();
                }
            }

            if (companies.Count > 0)
            {
                await _context.UKCompanies.AddRangeAsync(companies);
                await _context.SaveChangesAsync();
                totalImported += companies.Count;
            }

            Console.WriteLine($"Import completed: {totalImported:N0} companies");
        }

        private UKCompany? ParseCompanyFromCsv(string line)
        {
            var values = line.Split(',');
            if (values.Length < 6) return null;

            try
            {
                return new UKCompany
                {
                    CompanyName = values[0].Trim('"'),
                    CompanyNumber = values[1].Trim('"'),
                    Status = values[2].Trim('"'),
                    CompanyType = values[3].Trim('"'),
                    RegisteredAddress = values[4].Trim('"'),
                    PostCode = values[5].Trim('"')
                };
            }
            catch
            {
                return null;
            }
        }
    }
}