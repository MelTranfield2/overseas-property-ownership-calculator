using Microsoft.EntityFrameworkCore;
using LandValueTaxCalculator.Models;

namespace LandValueTaxCalculator.Data
{
    public class PropertyContext : DbContext
    {
        public PropertyContext(DbContextOptions<PropertyContext> options) : base(options) { }
        
        public DbSet<Property> Properties { get; set; }
        public DbSet<UKCompany> UKCompanies { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UKCompany>()
                .HasIndex(c => c.CompanyNumber)
                .IsUnique();
                
            modelBuilder.Entity<UKCompany>()
                .HasIndex(c => c.CompanyName);
        }
    }
}