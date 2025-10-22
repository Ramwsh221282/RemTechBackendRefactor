using FluentValidation;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.UseCases.Common;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.DependencyInjection;

public static class IdentityDomainInjection
{
    public static void InjectIdentityDomain(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(IdentityUser).Assembly);
        services.AddHandlersFromAssembly(typeof(IdentityUser).Assembly);
        services.AddScoped<IGetRoleByIdHandle, GetRoleByIdHandle>();
        services.AddScoped<IGetUserByIdHandle, GetUserByIdHandle>();
        services.AddScoped<IGetVerifiedUserHandle, GetVerifiedUserHandle>();
    }
}
