using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Mailing.Ports;

public interface IIdentityEmailSender
{
    Task<Status> SendEmail(
        IdentityMailingMessage message,
        UserEmail to,
        CancellationToken ct = default
    );
}
