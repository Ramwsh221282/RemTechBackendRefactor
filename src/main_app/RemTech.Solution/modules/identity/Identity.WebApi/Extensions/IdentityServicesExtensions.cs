using Identity.Domain.Accounts.Features.Dev_ChangePassword;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.WebApi.Extensions;

public static class IdentityServicesExtensions
{
    extension(IServiceProvider services)
    {
        public async Task<Result<Unit>> Dev_ResetPassword(string newPassword, Guid? id = null, string? login = null, string? email = null)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            Dev_ChangePasswordCommand command = new(newPassword, id, login, email);
            return await scope
                .ServiceProvider
                .GetRequiredService<ICommandHandler<Dev_ChangePasswordCommand, Unit>>()
                .Execute(command);
        }
    }
}