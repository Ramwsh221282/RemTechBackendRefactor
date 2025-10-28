using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Outbox;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.EfCore;

namespace ParsedAdvertisements.Adapters.Outbox;

public sealed class ParsedAdvertisementsOutboxDbContext(IOptions<DatabaseOptions> options) : DbContext
{
    public DbSet<ParsedAdvertisementsOutboxMessage> Messages => Set<ParsedAdvertisementsOutboxMessage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureForPgVector(options.Value.ToConnectionString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("parsed_advertisements_module");
        modelBuilder.ConfigureWithPgVectorExtension();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ParsedAdvertisementsOutboxDbContext).Assembly);
    }
}