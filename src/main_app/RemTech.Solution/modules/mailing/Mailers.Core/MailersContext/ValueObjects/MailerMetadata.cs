using Mailers.Communication.Abstractions;

namespace Mailers.Core.MailersContext.ValueObjects;

public sealed class MailerMetadata
{
    private readonly Guid _id;
    private readonly string _email;
    private readonly string _hashedSmtpPassword;
    
    private MailerMetadata(string email, string hashedSmtpPassword, Guid? id = null)
    {
        _id = id ?? Guid.NewGuid();
        _email = email;
        _hashedSmtpPassword = hashedSmtpPassword;
    }

    public void SignRegistration(MailerEvent @event)
    {
        @event.Accept(_id, _email, _hashedSmtpPassword);
    }

    public static Result<MailerMetadata> Create(string email, string hashedSmtpPassword, Guid? id = null)
    {
        return Invariant
            .For(email, Strings.NotEmptyOrWhiteSpace)
            .BindError(Validation("Почта не указана."))
            .SwitchTo(email, val => Strings.NotGreaterThan(val, 256))
            .BindError(Validation("Почта невалидна."))
            .SwitchTo(email, StringIsEmail)
            .BindError(Validation("Почта невалидна."))
            .SwitchTo(hashedSmtpPassword, Strings.NotEmptyOrWhiteSpace)
            .BindError(Validation("SMTP ключ не указан."))
            .Map(() => new MailerMetadata(email, hashedSmtpPassword, id));
    }

        private static bool StringIsEmail(string email) =>
            !string.IsNullOrWhiteSpace(email) &&
            System.Text.RegularExpressions.Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"
            );

    public Result<MailerMetadata> ChangeEmail(string email) =>
        Create(email, _hashedSmtpPassword, _id);
}