using Microsoft.EntityFrameworkCore;
using RemTech.Infrastructure.PostgreSQL;
using Vehicles.Domain.BrandContext;

namespace Vehicles.Infrastructure.PostgreSQL;

public sealed class VehiclesServiceDbContext : DbContext
{
    private readonly NpgsqlOptions _options;

    public VehiclesServiceDbContext(NpgsqlOptions options) => _options = options;

    public DbSet<Brand> Brands => Set<Brand>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UsePgVector(_options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UsePgVectorExtension();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VehiclesServiceDbContext).Assembly);
    }
}
