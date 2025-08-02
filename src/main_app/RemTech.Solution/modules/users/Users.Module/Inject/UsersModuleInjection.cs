using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Users.Module.Models;

namespace Users.Module.Inject;

public static class UsersModuleInjection
{
    public static void InjectUsersModule(
        this IServiceCollection services,
        UsersModuleOptions options
    )
    {
        services.AddAuthorization();
        services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);
        services
            .AddIdentityCore<User>()
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddApiEndpoints();
        services.AddDbContext<UsersDbContext>(o =>
            o.UseNpgsql(options.Database.ToConnectionString())
        );
    }

    public static async Task ApplyUsersModuleMigrations(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        await using UsersDbContext context =
            scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        await context.Database.MigrateAsync();
    }

    public static void MapIdentityApi(this WebApplication app) => app.MapIdentityApi<User>();
}
