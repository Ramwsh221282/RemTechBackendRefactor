using Mailers.Core.EmailsModule;
using Mailers.Core.MailedMessagesModule;

namespace Mailers.Core.MailersModule;

public sealed record MailerDeliveryArgs(Email To, string Subject, string Body);

public sealed record Mailer
{
    public MailerMetadata Metadata { get; init; }
    public SmtpPassword Password { get; init; }
    public MailerStatistics Statistics { get; init; }

    internal Mailer(MailerMetadata metadata, SmtpPassword password, MailerStatistics statistics)
    {
        Metadata = metadata;
        Password = password;
        Statistics = statistics;
    }

    public Result<Mailer> WithOtherPassword(string password)
    {
        Result<SmtpPassword> nextPassword = SmtpPassword.Construct(password);
        if (nextPassword.IsFailure) return nextPassword.Error;
        return WithOtherPassword(nextPassword);
    }
    
    public Mailer WithOtherPassword(SmtpPassword password)
    {
        return this with { Password = password };
    }
    
    public Result<MailerSending> SendEmail(MailerDeliveryArgs args)
    {
        if (string.IsNullOrWhiteSpace(args.Subject))
            return Conflict("Невозможно отправить сообщение. Тема письма не указана.");

        if (string.IsNullOrWhiteSpace(args.Body))
            return Conflict("Невозможно отправить сообщение. Тело письма не указано.");

        MailedMessageMetadata metadata = new(Metadata.Id.Value, Guid.NewGuid());
        MailedMessageDeliveryInfo delivery = new(args.To, DateTime.UtcNow);
        MailedMessageContent content = new(args.Subject, args.Body);
        MailedMessage message = new(metadata, content, delivery);
        return new MailerSending(this with { Statistics = Statistics.Increased() }, message);
    }
}