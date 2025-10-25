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
        services.AddValidatorsFromAssembly(typeof(User).Assembly);
        services.AddHandlersFromAssembly(typeof(User).Assembly);
        services.AddScoped<IGetRoleByIdHandle, GetRoleByIdHandle>();
        services.AddScoped<IGetUserByIdHandle, GetUserByIdHandle>();
        services.AddScoped<IGetVerifiedUserHandle, GetVerifiedUserHandle>();
        services.AddScoped<IGetUserByTicketHandle, GetUserByTicketHandle>();
        services.AddScoped<IGetUserByEmailHandle, GetUserByEmailHandle>();
        services.AddScoped<IGetUserByLoginHandle, GetUserByLoginHandle>();
        services.AddScoped<IGetUserHandle, MultiWayGetUserHandle>();
        services.AddScoped<IManageUserProfileUniqueAttributes, ManageUserProfileUniqueAttributes>();
        services.AddScoped<IUserEmailUnique, UserEmailUnique>();
        services.AddScoped<IUserLoginUnique, UserLoginUnique>();
    }
}
