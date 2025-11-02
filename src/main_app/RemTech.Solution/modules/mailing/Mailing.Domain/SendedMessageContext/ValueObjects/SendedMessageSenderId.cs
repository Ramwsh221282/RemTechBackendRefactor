using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.SendedMessageContext.ValueObjects;

public sealed record SendedMessageSenderId
{
    public Guid Value { get; }

    private SendedMessageSenderId(Guid value) => Value = value;

    public static Status<SendedMessageSenderId> Create(Guid value) =>
        from valid_id in NotEmptyGuid.New(value)
            .OverrideValidationError("У отправленного письма, идентификатор отправителя не может быть пустым.")
        select new SendedMessageSenderId(valid_id);
}