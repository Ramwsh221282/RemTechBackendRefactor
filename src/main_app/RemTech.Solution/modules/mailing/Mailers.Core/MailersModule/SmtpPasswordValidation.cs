namespace Mailers.Core.MailersModule;

public static class SmtpPasswordValidation
{
    extension(SmtpPassword password)
    {
        public Result<SmtpPassword> Validated()
        {
            if (password.IsEmptyOrWhiteSpace()) return Validation("SMTP пароль не указан.");
            if (password.GreaterThan(256)) return Validation("SMTP пароль превышает 256 символов.");
            return password;
        }

        private bool IsEmptyOrWhiteSpace()
        {
            string value = password.Value;
            return string.IsNullOrWhiteSpace(value);
        }

        private bool GreaterThan(int maxLength)
        {
            string value =  password.Value;
            return value.Length > maxLength;
        }
    }
}