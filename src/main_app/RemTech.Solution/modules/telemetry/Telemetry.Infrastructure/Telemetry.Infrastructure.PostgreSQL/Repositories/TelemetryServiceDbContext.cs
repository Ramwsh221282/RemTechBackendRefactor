using Microsoft.EntityFrameworkCore;
using Telemetry.Domain.TelemetryContext;

namespace Telemetry.Infrastructure.PostgreSQL.Repositories;

public sealed class TelemetryServiceDbContext : DbContext
{
    private readonly PostgreSqlConnectionOptions _options;

    public TelemetryServiceDbContext(PostgreSqlConnectionOptions options) => _options = options;

    public DbSet<TelemetryRecord> Records => Set<TelemetryRecord>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_options.FormConnectionString(), o => o.UseVector());
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.HasDefaultSchema("telemetry_module");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TelemetryServiceDbContext).Assembly);
    }
}
