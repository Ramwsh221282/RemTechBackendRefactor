using System.Text.RegularExpressions;
using RemTech.Primitives.Extensions.Exceptions;

namespace Mailing.Core.Common;

public static partial class EmailValidation
{
    private const int MaxLength = 256;
    
    extension(Email email)
    {
        public void Validate()
        {
            string value = email.Value;
            const string valueName = "Почта получателя";
            if (string.IsNullOrWhiteSpace(value)) throw ErrorException.ValueNotSet(valueName);
            if (value.Length > MaxLength) throw ErrorException.ValueExcess(valueName, MaxLength);
            if (!EmailRegex().IsMatch(value)) throw ErrorException.ValueInvalidFormat(valueName);
        }
    }
    
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}