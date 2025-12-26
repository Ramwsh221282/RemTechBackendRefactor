using Identity.Adapter.Storage.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;

namespace Identity.Adapter.Storage;

public sealed class IdentityDbContext(IOptions<DatabaseOptions> options) : DbContext
{
    public DbSet<IdentityUserDataModel> Users => Set<IdentityUserDataModel>();
    public DbSet<IdentityRoleDataModel> Roles => Set<IdentityRoleDataModel>();
    public DbSet<IdentityUserRoleDataModel> UserRoles => Set<IdentityUserRoleDataModel>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(options.Value.ToConnectionString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("users_module");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}
