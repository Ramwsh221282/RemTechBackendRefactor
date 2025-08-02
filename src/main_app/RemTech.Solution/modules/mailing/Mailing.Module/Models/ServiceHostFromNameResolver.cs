namespace Mailing.Module.Models;

internal sealed class ServiceHostFromNameResolver(string senderName)
{
    private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    public string Resolve()
    {
        return senderName switch
        {
            null => throw new ArgumentException("Email sender name is null."),
            not null when string.IsNullOrWhiteSpace(senderName) => throw new ArgumentException(
                "Email sender is empty."
            ),
            not null when senderName.Contains("mail.ru", Comparison) => string.Intern(
                "smtp.mail.ru"
            ),
            not null when senderName.Contains("gmail.com", Comparison) => string.Intern(
                "smtp.gmail.com"
            ),
            not null when senderName.Contains("yandex.ru", Comparison) => string.Intern(
                "smtp.yandex.ru"
            ),
            _ => throw new ArgumentException("Email sender host server is not supported."),
        };
    }
}
