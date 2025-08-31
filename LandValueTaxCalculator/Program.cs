using LandValueTaxCalculator.Data;
using LandValueTaxCalculator.Services;
using LandValueTaxCalculator.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add PostgreSQL
builder.Services.AddDbContext<PropertyContext>(options =>
    options.UseInMemoryDatabase("LandValueTaxDB"));
    // options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<ITaxCalculationService, TaxCalculationService>();

var app = builder.Build();

// Create database and load data on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PropertyContext>();
    var propertyService = scope.ServiceProvider.GetRequiredService<IPropertyService>();
    
    await context.Database.EnsureCreatedAsync();
    Console.WriteLine("Database created successfully");
    
    // Load overseas properties if empty
    var propertyCount = await context.Properties.CountAsync();
    if (propertyCount == 0)
    {
        Console.WriteLine("Loading overseas properties...");
        await LoadOverseasPropertiesAsync(context, "overseas_properties.csv");
    }
    
    // Load UK companies if empty  
    var companyCount = await context.UKCompanies.CountAsync();
    if (companyCount == 0)
    {
        Console.WriteLine("Loading UK companies...");
        await LoadUKCompaniesAsync(context, "test-uk-companies.csv");
    }
    
    Console.WriteLine($"Database ready: {await context.Properties.CountAsync()} properties, {await context.UKCompanies.CountAsync()} companies");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Helper method to load overseas properties
async Task LoadOverseasPropertiesAsync(PropertyContext context, string csvPath)
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
                PurchaseDate = DateTime.Now.AddYears(-2)
            };
            properties.Add(property);
        }
    }
    
    await context.Properties.AddRangeAsync(properties);
    await context.SaveChangesAsync();
    Console.WriteLine($"Loaded {properties.Count} overseas properties");
}

// Helper method to load UK companies
async Task LoadUKCompaniesAsync(PropertyContext context, string csvPath)
{
    if (!File.Exists(csvPath))
    {
        Console.WriteLine($"CSV file not found: {csvPath}");
        return;
    }

    const int batchSize = 1000;
    var companies = new List<UKCompany>();
    var totalLoaded = 0;
    
    var lines = await File.ReadAllLinesAsync(csvPath);
    for (int i = 1; i < lines.Length; i++)
    {
        var line = lines[i];
    // }
    // {
        var values = line.Split(',');
        if (values.Length >= 6)
        {
            companies.Add(new UKCompany
            {
                CompanyName = values[0].Trim('"'),
                CompanyNumber = values[1].Trim('"'),
                Status = values[2].Trim('"'),
                CompanyType = values[3].Trim('"'),
                RegisteredAddress = values[4].Trim('"'),
                PostCode = values[5].Trim('"')
            });
        }

        if (companies.Count >= batchSize)
        {
            await context.UKCompanies.AddRangeAsync(companies);
            await context.SaveChangesAsync();
            totalLoaded += companies.Count;
            Console.WriteLine($"Loaded {totalLoaded} companies...");
            companies.Clear();
            context.ChangeTracker.Clear();
        }
    }
    
    if (companies.Count > 0)
    {
        await context.UKCompanies.AddRangeAsync(companies);
        await context.SaveChangesAsync();
        totalLoaded += companies.Count;
    }
    
    Console.WriteLine($"Finished loading {totalLoaded} UK companies");
}

string[] ParseCsvLine(string line)
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

decimal EstimateValue(string region) => region?.ToUpper() switch
{
    "GREATER LONDON" => 1000000,
    "SOUTH EAST" => 500000,
    _ => 300000
};

double GetLatitude(string region) => region?.ToUpper() switch
{
    "GREATER LONDON" => 51.5074,
    "NORTH WEST" => 53.4808,
    _ => 52.3555
};

double GetLongitude(string region) => region?.ToUpper() switch
{
    "GREATER LONDON" => -0.1278,
    "NORTH WEST" => -2.2426,
    _ => -1.1743
};