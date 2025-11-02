using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.SendedMessageContext.ValueObjects;

public readonly record struct SendedMessageId
{
    public Guid Value { get; }

    private SendedMessageId(Guid value) => Value = value;

    public static Status<SendedMessageId> Create(Guid value) =>
        from valid_id in NotEmptyGuid.New(value)
            .OverrideValidationError("У отправленного письма, идентификатор не может быть пустым.")
        select new SendedMessageId(valid_id);
}