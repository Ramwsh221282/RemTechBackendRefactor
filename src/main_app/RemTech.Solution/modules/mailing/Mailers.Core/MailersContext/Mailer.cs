namespace Mailers.Core.MailersContext;

public sealed record Mailer(MailerMetadata Metadata, MailerStatistics Statistics)
{
    public Result<MailerSending> SendEmail(Email to, string subject, string body)
    {
        if (Statistics.LimitReached())
            return Conflict("Невозможно отправить сообщение. Превышен лимит отправки данным сервисом.");

        if (string.IsNullOrWhiteSpace(subject))
            return Conflict("Невозможно отправить сообщение. Тема письма не указана.");

        if (string.IsNullOrWhiteSpace(body))
            return Conflict("Невозможно отправить сообщение. Тело письма не указано.");

        MailedMessageMetadata metadata = new(Metadata.Id, Guid.NewGuid());
        MailedMessageDeliveryInfo delivery = new(to, DateTime.UtcNow);
        MailedMessageContent content = new(subject, body);
        MailedMessage message = new(metadata, content, delivery);
        MailerStatistics increased = Statistics.Increased();
        return new MailerSending(this with { Statistics =  increased }, message);
    }
}