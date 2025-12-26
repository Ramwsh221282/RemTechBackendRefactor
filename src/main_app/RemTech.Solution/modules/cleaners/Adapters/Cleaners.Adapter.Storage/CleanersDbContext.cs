using Cleaners.Adapter.Storage.DataModels;
using Cleaners.Domain.Cleaners.Ports.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;

namespace Cleaners.Adapter.Storage;

public sealed class CleanersDbContext(IOptions<DatabaseOptions> options) : DbContext
{
    public DbSet<CleanerDataModel> Cleaners => Set<CleanerDataModel>();
    public DbSet<CleanerOutboxMessage> Outbox => Set<CleanerOutboxMessage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(options.Value.ToConnectionString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cleaners_module");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CleanersDbContext).Assembly);
    }
}
