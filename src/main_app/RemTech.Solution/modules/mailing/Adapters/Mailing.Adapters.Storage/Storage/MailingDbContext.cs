using Mailing.Adapters.Storage.PostedMessages;
using Mailing.Adapters.Storage.Postmans;
using Mailing.Adapters.Storage.PostmanStatistics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.EfCore;

namespace Mailing.Adapters.Storage.Storage;

internal sealed class MailingDbContext(IOptions<DatabaseOptions> options) : DbContext
{
    public DbSet<DbPostman> Postmans => Set<DbPostman>();
    public DbSet<DbPostedMessage> Messages => Set<DbPostedMessage>();
    public DbSet<DbPostmanStatistics> Statistics => Set<DbPostmanStatistics>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureForPgVector(options.Value.ToConnectionString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("mailing_module");
        modelBuilder.ConfigureWithPgVectorExtension();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MailingDbContext).Assembly);
    }
}