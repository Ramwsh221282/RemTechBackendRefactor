using CompositionRoot.Shared;
using Identity.Core.SubjectsModule.Contracts;
using Identity.PasswordHash;
using Microsoft.Extensions.DependencyInjection;

namespace CompositionRoot.identity.DependencyInjection.IdentityPasswordHashing;

[DependencyInjectionClass]
internal static class IdentityPasswordHashingInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddOptions<IdentityPasswordHashOptions>()
            .BindConfiguration(nameof(IdentityPasswordHashOptions));
        
        services.AddTransient<HashPassword>(sp => sp.ResolveHashPasswordFunction());
    }
}