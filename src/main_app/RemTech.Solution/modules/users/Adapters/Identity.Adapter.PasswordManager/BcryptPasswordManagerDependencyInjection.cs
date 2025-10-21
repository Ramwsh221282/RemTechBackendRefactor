using Identity.Domain.Users.Ports.Passwords;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Adapter.PasswordManager;

public static class BcryptPasswordManagerDependencyInjection
{
    public static void InjectBcryptPasswordManager(this IServiceCollection services) =>
        services.AddSingleton<IPasswordManager, BcryptPasswordManager>();
}
