using Mailing.Domain.EmailSendingContext.ValueObjects;
using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.EmailSendingContext;

public static class EmailSenderInformationFactory
{
    public static Status<SenderServiceInformation> Create(
        Func<Status<NotEmptyString>> serviceDispatch,
        Func<Status<NotEmptyString>> passwordHash) =>
        from service_name in serviceDispatch()
        from password in passwordHash()
        select SenderServiceInformation.Create(service_name, password);
}