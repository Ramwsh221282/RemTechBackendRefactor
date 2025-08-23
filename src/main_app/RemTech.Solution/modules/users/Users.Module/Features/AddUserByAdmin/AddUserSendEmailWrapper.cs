using Mailing.Module.Bus;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Frontend;
using Users.Module.Features.ChangingEmail.Exceptions;

namespace Users.Module.Features.AddUserByAdmin;

internal sealed class AddUserSendEmailWrapper(
    MailingBusPublisher publisher,
    FrontendUrl frontendUrl,
    ConfirmationEmailsCache cache,
    ICommandHandler<AddUserByAdminCommand, AddUserByAdminResult> origin
) : ICommandHandler<AddUserByAdminCommand, AddUserByAdminResult>
{
    public async Task<AddUserByAdminResult> Handle(
        AddUserByAdminCommand command,
        CancellationToken ct = default
    )
    {
        AddUserByAdminResult result = await origin.Handle(command, ct);
        Guid key = Guid.NewGuid();
        await cache.Create(key, result.Id);
        await new AddUserByAdminEmailMessage(result.Password, key, frontendUrl, publisher).Send(
            result.Email,
            ct
        );
        return result;
    }
}
