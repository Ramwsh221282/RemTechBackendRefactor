using Mailing.Domain.EmailSendingContext.Ports;
using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.EmailSendingContext;

public sealed class EmailSenderServicePasswordHash(Status<NotEmptyString> password)
{
    public EmailSenderServicePasswordHash(string password) : this(NotEmptyString.New(password))
    {
    }

    public Status<NotEmptyString> Read(ICanCypherString hash) =>
        from not_hashed in password
        from hashed in hash.Hash(not_hashed).AsSuccessStatus()
        select NotEmptyString.New(hashed);
}