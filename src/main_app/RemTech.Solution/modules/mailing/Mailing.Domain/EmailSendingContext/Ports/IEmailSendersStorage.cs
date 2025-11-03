using System.Data;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.EmailSendingContext.Ports;

public interface IEmailSendersStorage
{
    Task<Status<Unit>> Accept(Action<IDbCommand> command, CancellationToken ct = default);
}