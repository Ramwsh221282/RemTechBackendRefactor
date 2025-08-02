namespace Mailing.Module.Models;

internal sealed class ServiceHostFromNameResolver(string senderName)
{
    private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    public string Resolved() =>
        senderName switch
        {
            null => throw new ArgumentException("Email sender name is null."),
            not null when string.IsNullOrWhiteSpace(senderName) => throw new ArgumentException(
                "Email sender is empty."
            ),
            not null when senderName.EndsWith("mail.ru", Comparison) => string.Intern(
                "smtp.mail.ru"
            ),
            not null when senderName.EndsWith("gmail.com", Comparison) => string.Intern(
                "smtp.gmail.com"
            ),
            not null when senderName.EndsWith("yandex.ru", Comparison) => string.Intern(
                "smtp.yandex.ru"
            ),
            _ => throw new ArgumentException("Email sender host server is not supported."),
        };
}
