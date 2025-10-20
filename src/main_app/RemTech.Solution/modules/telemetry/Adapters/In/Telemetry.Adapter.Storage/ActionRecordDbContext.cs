using Microsoft.EntityFrameworkCore;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.EfCore;
using Shared.Infrastructure.Module.Postgres;
using Telemetry.Adapter.Storage.DataModels;

namespace Telemetry.Adapter.Storage;

public sealed class ActionRecordDbContext(DatabaseOptions configuration) : DbContext, IStorageUpper
{
    public DbSet<ActionRecordDataModel> Records => Set<ActionRecordDataModel>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.ConfigureForPgVector(configuration.ToConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ConfigureWithPgVectorExtension(
            b => b.HasDefaultSchema("telemetry_module"),
            b => b.ApplyConfigurationsFromAssembly(typeof(ActionRecordDbContext).Assembly)
        );

    public async Task UpStorage()
    {
        await Database.MigrateAsync();
    }
}
