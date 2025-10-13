using Microsoft.EntityFrameworkCore;
using RemTech.Infrastructure.PostgreSQL;
using Telemetry.Domain.TelemetryContext;

namespace Telemetry.Infrastructure.PostgreSQL.Repositories;

public sealed class TelemetryServiceDbContext : DbContext
{
    private readonly NpgsqlOptions _options;

    public TelemetryServiceDbContext(NpgsqlOptions options) => _options = options;

    public DbSet<TelemetryRecord> Records => Set<TelemetryRecord>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // подключение postgre с расширением pgvector
        optionsBuilder.UsePgVector(_options);
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // подключение postgre с расширением pgvector
        modelBuilder.UsePgVectorExtension();
        // задача отдельной схемы
        modelBuilder.HasDefaultSchema("telemetry_module");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TelemetryServiceDbContext).Assembly);
    }
}
