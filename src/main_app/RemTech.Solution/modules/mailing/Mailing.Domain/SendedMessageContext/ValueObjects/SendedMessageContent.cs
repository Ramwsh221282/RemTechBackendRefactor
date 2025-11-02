using Mailing.Domain.EmailSendingContext.ValueObjects;
using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.SendedMessageContext.ValueObjects;

public sealed record SendedMessageContent
{
    public EmailString RecipientEmail { get; }
    public string Subject { get; }
    public string Body { get; }

    private SendedMessageContent(EmailString recipientEmail, string subject, string body)
    {
        RecipientEmail = recipientEmail;
        Subject = subject;
        Body = body;
    }

    public static Status<SendedMessageContent> Create(EmailString recipientEmail, string subject, string body) =>
        from valid_subject in NotEmptyString.New(subject).OverrideValidationError("Тема письма не указана.")
        from valid_body in NotEmptyString.New(body).OverrideValidationError("Тема письма не указана.")
        select new SendedMessageContent(recipientEmail, valid_subject, valid_body);
}