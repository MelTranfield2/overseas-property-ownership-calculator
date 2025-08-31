using LandValueTaxCalculator.Data;
using LandValueTaxCalculator.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<PropertyContext>(options =>
    options.UseInMemoryDatabase("LandValueTaxDB"));
    // For production, uncomment below and use PostgreSQL:
    // options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<ITaxCalculationService, TaxCalculationService>();
builder.Services.AddScoped<DataImportService>();

var app = builder.Build();

// Create database and load data on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PropertyContext>();
    var dataImportService = scope.ServiceProvider.GetRequiredService<DataImportService>();
    
    await context.Database.EnsureCreatedAsync();
    Console.WriteLine("Database created successfully");
    
    // Load properties if empty
    var propertyCount = await context.Properties.CountAsync();
    if (propertyCount == 0)
    {
        Console.WriteLine("Loading overseas properties...");
        await dataImportService.ImportOverseasPropertiesAsync("overseas_properties.csv");
        
        Console.WriteLine("Loading UK properties...");
        await dataImportService.ImportUKPropertiesAsync("test-uk-companies.csv");
    }
    
    var totalProperties = await context.Properties.CountAsync();
    var overseasProperties = await context.Properties.CountAsync(p => !p.IsUKCompany);
    var ukProperties = await context.Properties.CountAsync(p => p.IsUKCompany);
    
    Console.WriteLine($"Database ready: {totalProperties} total properties");
    Console.WriteLine($"  - {overseasProperties} overseas-owned properties");
    Console.WriteLine($"  - {ukProperties} UK company-owned properties");
}

// Configure the HTTP request pipeline
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