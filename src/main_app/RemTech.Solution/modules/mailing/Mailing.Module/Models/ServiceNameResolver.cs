namespace Mailing.Module.Models;

internal sealed class ServiceNameResolver(string email)
{
    private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    public string Resolved() =>
        email switch
        {
            null => throw new ArgumentException("Email sender name is null."),
            not null when string.IsNullOrWhiteSpace(email) => throw new ArgumentException(
                "Email sender is empty."
            ),
            not null when email.EndsWith("mail.ru", Comparison) => string.Intern("mail.ru"),
            not null when email.EndsWith("gmail.com", Comparison) => string.Intern("gmail.com"),
            not null when email.EndsWith("yandex.ru", Comparison) => string.Intern("yandex.ru"),
            _ => throw new ArgumentException("Email sender service is not supported."),
        };
}
