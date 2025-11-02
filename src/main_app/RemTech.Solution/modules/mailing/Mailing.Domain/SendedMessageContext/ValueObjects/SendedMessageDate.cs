using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.SendedMessageContext.ValueObjects;

public readonly record struct SendedMessageDate
{
    public DateTime Value { get; }

    public SendedMessageDate() => Value = DateTime.UtcNow;

    private SendedMessageDate(DateTime value) => Value = value;

    public static Status<SendedMessageDate> Create(DateTime value) =>
        from valid_date in NotEmptyDateTime.Create(value)
            .OverrideValidationError("Дата отправки сообщения некорректна.")
        select new SendedMessageDate(value);
}