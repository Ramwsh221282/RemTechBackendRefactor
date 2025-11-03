using System.Data;
using Mailing.Domain.EmailSendingContext.Ports;
using RemTech.Core.Shared.Database;
using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.EmailSendingContext.ValueObjects;

public readonly struct EmailSenderId
{
    private readonly Guid _value;

    public EmailSenderId() => _value = Guid.NewGuid();

    private EmailSenderId(Guid value) => _value = value;

    public static Status<EmailSenderId> Create(Guid value) =>
        from valid_id in NotEmptyGuid.New(value)
            .OverrideValidationError("Идентификатор отправителя почты не может быть пустым.")
        select new EmailSenderId(valid_id);

    public IDbCommand AppendParameter(IDbCommand command)
    {
        command.AddParameter("@id", _value);
        return command;
    }
}