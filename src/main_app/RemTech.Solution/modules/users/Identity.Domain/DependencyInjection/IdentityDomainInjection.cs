using FluentValidation;
using Identity.Domain.Users.Aggregate;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.DependencyInjection;

public static class IdentityDomainInjection
{
    public static void InjectIdentityDomain(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(IdentityUser).Assembly);
        services.AddHandlersFromAssembly(typeof(IdentityUser).Assembly);
    }
}
