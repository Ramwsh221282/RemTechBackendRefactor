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
        // подключение postgre с расширением pgvector
        optionsBuilder.UseNpgsql(_options.FormConnectionString(), o => o.UseVector());
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // подключение postgre с расширением pgvector
        modelBuilder.HasPostgresExtension("vector");
        // задача отдельной схемы
        modelBuilder.HasDefaultSchema("telemetry_module");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TelemetryServiceDbContext).Assembly);
    }
}
