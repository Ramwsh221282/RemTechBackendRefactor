using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Emails;

public sealed class EmailString
{
    private static readonly Error _invalidEmail = Error.Validation("Формат почты некорректный.");
    private static readonly Error _emptyEmail = Error.Validation("Почта была пустой.");
    private readonly string _value;

    public EmailString(string value) => _value = value;

    public bool OfCorrectFormat(out Error error)
    {
        error = Error.None();
        if (!IsNotEmpty())
        {
            error = _emptyEmail;
            return false;
        }

        if (!ContainsValidDomain())
        {
            error = _invalidEmail;
            return false;
        }

        if (!ContainsSeparator())
        {
            error = _emptyEmail;
            return false;
        }

        return true;
    }

    private bool ContainsValidDomain()
    {
        int atIndex = _value.IndexOf('@');
        string domain = _value[(atIndex + 1)..];
        return domain.Contains('.') && domain.IndexOf('.') != domain.Length - 1;
    }

    private bool ContainsSeparator()
    {
        int atIndex = _value.IndexOf('@');
        return atIndex > 0 && atIndex != _value.Length - 1;
    }

    private bool IsNotEmpty()
    {
        return !string.IsNullOrWhiteSpace(_value);
    }
}