using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ParsedAdvertisements.Adapters.Storage.BrandContext.DataModels;
using ParsedAdvertisements.Adapters.Storage.CategoryContext.Configurations;
using ParsedAdvertisements.Adapters.Storage.ModelContext.DataModels;
using ParsedAdvertisements.Adapters.Storage.RegionContext.DataModels;
using ParsedAdvertisements.Adapters.Storage.VehicleContext.DataModels;
using ParsedAdvertisements.Domain.CharacteristicContext;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.EfCore;

namespace ParsedAdvertisements.Adapters.Storage;

public sealed class ParsedAdvertisementsDbContext(IOptions<DatabaseOptions> options) : DbContext
{
    public DbSet<VehicleDataModel> Vehicles => Set<VehicleDataModel>();
    public DbSet<VehicleCharacteristicDataModel> VehicleCharacteristics => Set<VehicleCharacteristicDataModel>();
    public DbSet<Characteristic> Characteristics => Set<Characteristic>();
    public DbSet<CategoryDataModel> Categories => Set<CategoryDataModel>();
    public DbSet<BrandDataModel> Brands => Set<BrandDataModel>();
    public DbSet<ModelDataModel> Models => Set<ModelDataModel>();
    public DbSet<RegionDataModel> Regions => Set<RegionDataModel>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureForPgVector(options.Value.ToConnectionString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("parsed_advertisements_module");
        modelBuilder.HasPostgresExtension("ltree");
        modelBuilder.ConfigureWithPgVectorExtension();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ParsedAdvertisementsDbContext).Assembly);
    }
}