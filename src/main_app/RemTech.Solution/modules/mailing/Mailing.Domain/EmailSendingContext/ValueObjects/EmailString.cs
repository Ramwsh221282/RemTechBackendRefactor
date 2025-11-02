using System.Text.RegularExpressions;
using Mailing.Domain.EmailSendingContext.Ports;
using RemTech.Core.Shared.Monads;
using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.EmailSendingContext.ValueObjects;

public sealed class EmailString
{
    private const RegexOptions EmailRegexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;

    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        EmailRegexOptions);

    private readonly string _value;

    private EmailString(string value) => _value = value;

    public static Status<EmailString> Create(string value) =>
        from valid_string in NotEmptyString.New(value)
        from valid_email in new MatchedRegexString(value, EmailRegex).Matched(
            Error.Validation($"Некорректная строка почты: {value}"))
        select new EmailString(value);

    public T Fold<T>(EmailSenderEmailSink<T> use) => use(_value);
}