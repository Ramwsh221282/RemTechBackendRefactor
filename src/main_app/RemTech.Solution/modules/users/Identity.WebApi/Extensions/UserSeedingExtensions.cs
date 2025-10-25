using Identity.Adapter.Storage;
using Identity.Adapter.Storage.Seeding;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Identity.WebApi.Extensions;

public static class UserSeedingExtensions
{
    public static async Task SeedUser(this IHost host, string email, string login, string password)
    {
        await using var scope = host.Services.CreateAsyncScope();
        var seeder = scope.GetService<IUserSeeding>();
        await seeder.SeedUser(email, login, password);
    }

    public static async Task MigrateIdentityModule(this IHost host)
    {
        await using var scope = host.Services.CreateAsyncScope();
        await using var context = scope.GetService<IdentityDbContext>();
        await context.Database.MigrateAsync();
    }

    public static async Task AddRoles(this IHost host, params string[] roles)
    {
        await using var scope = host.Services.CreateAsyncScope();
        var seeder = scope.GetService<IRolesSeeding>();
        foreach (var role in roles)
            seeder.AddRole(role);
        await seeder.Seed();
    }
}
