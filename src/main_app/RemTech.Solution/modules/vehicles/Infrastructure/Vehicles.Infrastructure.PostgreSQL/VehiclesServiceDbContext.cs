using Microsoft.EntityFrameworkCore;
using RemTech.Infrastructure.PostgreSQL;
using Vehicles.Domain.BrandContext;
using Vehicles.Domain.CategoryContext;
using Vehicles.Domain.LocationContext;
using Vehicles.Domain.ModelContext;
using Vehicles.Domain.VehicleContext;

namespace Vehicles.Infrastructure.PostgreSQL;

public sealed class VehiclesServiceDbContext : DbContext
{
    private readonly NpgsqlOptions _options;

    public VehiclesServiceDbContext(NpgsqlOptions options) => _options = options;

    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<VehicleModel> Models => Set<VehicleModel>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UsePgVector(_options);
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("vehicles_module");
        modelBuilder.UsePgVectorExtension();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VehiclesServiceDbContext).Assembly);
    }
}
