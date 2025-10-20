using Mailing.Module.Bus;
using Microsoft.Extensions.Options;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using RemTech.Shared.Configuration.Options;
using Users.Module.Features.ChangingEmail;

namespace Users.Module.Features.AddUserByAdmin;

internal sealed class AddUserSendEmailWrapper(
    MailingBusPublisher publisher,
    IOptions<FrontendOptions> frontendUrl,
    ConfirmationEmailsCache cache,
    ICommandHandler<AddUserByAdminCommand, Status<AddUserByAdminResult>> origin
) : ICommandHandler<AddUserByAdminCommand, Status<AddUserByAdminResult>>
{
    public async Task<Status<AddUserByAdminResult>> Handle(
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
