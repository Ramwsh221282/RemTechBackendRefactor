using System.Text.RegularExpressions;

namespace Mailers.Core.EmailsModule;

public static partial class EmailsValidation
{
    extension(Email email)
    {
        public Result<Email> Validated()
        {
            if (email.IsNullOrEmpty()) return Validation("Почта не указана.");
            if (!email.HasValidFormat()) return Validation("Некорректный формат почты.");
            return email;
        }

        private bool IsNullOrEmpty()
        {
            return string.IsNullOrWhiteSpace(email.Value);
        }

        private bool HasValidFormat()
        {
            string value = email.Value;
            return value.Length <= 256 && MatchesEmailRegex(value);
        }
    }
    
    private static bool MatchesEmailRegex(string input)
    {
        return EmailRegex().IsMatch(input);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}