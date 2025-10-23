using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.EfCore;
using Tickets.Adapter.Storage.DataModels;

namespace Tickets.Adapter.Storage.Implementations;

public sealed class TicketsDbContext(IOptions<DatabaseOptions> options) : DbContext
{
    public DbSet<TicketDataModel> Tickets { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureForPgVector(options.Value.ToConnectionString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureWithPgVectorExtension();
        modelBuilder.HasDefaultSchema("tickets_module");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketsDbContext).Assembly);
    }
}
