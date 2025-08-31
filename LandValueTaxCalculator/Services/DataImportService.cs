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

        // Import overseas properties (properties owned by overseas companies)
        public async Task ImportOverseasPropertiesAsync(string csvPath)
        {
            if (!File.Exists(csvPath))
            {
                Console.WriteLine($"CSV file not found: {csvPath}");
                return;
            }

            var properties = new List<Property>();
            var lines = await File.ReadAllLinesAsync(csvPath);

            for (int i = 1; i < lines.Length; i++)
            {
                var values = ParseCsvLine(lines[i]);
                if (values.Length >= 13)
                {
                    var property = new Property
                    {
                        Address = values[2],
                        Region = values[5],
                        PostCode = values[6],
                        EstimatedValue = decimal.TryParse(values[8], out var price) ? price : EstimateValue(values[5]),
                        OwnerCompany = values[9],
                        OwnerCountry = values[12],
                        Latitude = GetLatitude(values[5]),
                        Longitude = GetLongitude(values[5]),
                        PurchaseDate = DateTime.Now.AddYears(-2),
                        IsUKCompany = false, // These are owned by overseas companies
                        CompanyType = values[11]
                    };
                    properties.Add(property);
                }
            }

            await _context.Properties.AddRangeAsync(properties);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Loaded {properties.Count} overseas properties");
        }

        // Import UK properties (properties owned by UK companies)
        public async Task ImportUKPropertiesAsync(string csvPath)
        {
            if (!File.Exists(csvPath))
            {
                Console.WriteLine($"CSV file not found: {csvPath}");
                return;
            }

            var properties = new List<Property>();
            var lines = await File.ReadAllLinesAsync(csvPath);

            for (int i = 1; i < lines.Length; i++)
            {
                var values = ParseCsvLine(lines[i]);
                if (values.Length >= 13)
                {
                    var property = new Property
                    {
                        Address = values[2], // Property Address
                        Region = values[5], // Region  
                        PostCode = values[6], // Postcode
                        EstimatedValue = decimal.TryParse(values[8], out var price) ? price : EstimateValue(values[5]),
                        OwnerCompany = values[9], // Proprietor Name (1)
                        OwnerCountry = "United Kingdom", // These are UK companies
                        Latitude = GetLatitude(values[5]),
                        Longitude = GetLongitude(values[5]),
                        PurchaseDate = DateTime.Now.AddYears(-1),
                        IsUKCompany = true, // Mark as UK company-owned
                        CompanyType = values[11] // Proprietorship Category (1)
                    };
                    properties.Add(property);
                }
            }

            await _context.Properties.AddRangeAsync(properties);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Loaded {properties.Count} UK properties");
        }

        // Helper methods
        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var inQuotes = false;
            var currentField = "";

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '"') inQuotes = !inQuotes;
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentField.Trim());
                    currentField = "";
                }
                else currentField += c;
            }

            result.Add(currentField.Trim());
            return result.ToArray();
        }

        private decimal EstimateValue(string region) => region?.ToUpper() switch
        {
            "GREATER LONDON" => 1000000,
            "SOUTH EAST" => 500000,
            _ => 300000
        };

        private double GetLatitude(string region) => region?.ToUpper() switch
        {
            "GREATER LONDON" => 51.5074,
            "NORTH WEST" => 53.4808,
            "WEST MIDLANDS" => 52.4862,
            "YORKS AND HUMBER" => 53.8008,
            "NORTH" => 54.9783,
            "SOUTH WEST" => 50.7184,
            "WALES" => 52.1307,
            "SCOTLAND" => 56.4907,
            _ => 52.3555
        };

        private double GetLongitude(string region) => region?.ToUpper() switch
        {
            "GREATER LONDON" => -0.1278,
            "NORTH WEST" => -2.2426,
            "WEST MIDLANDS" => -1.8904,
            "YORKS AND HUMBER" => -1.5491,
            "NORTH" => -1.6178,
            "SOUTH WEST" => -4.3781,
            "WALES" => -3.7837,
            "SCOTLAND" => -4.2026,
            _ => -1.1743
        };
    }
}