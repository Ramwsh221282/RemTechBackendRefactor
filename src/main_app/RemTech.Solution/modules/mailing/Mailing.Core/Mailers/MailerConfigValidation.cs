using RemTech.Primitives.Extensions.Exceptions;

namespace Mailing.Core.Mailers;

public static class MailerConfigValidation
{
    extension(MailerConfig config)
    {
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(config.SmtpPassword))
                throw ErrorException.ValueNotSet("SMTP ключ сервиса отправки email");
        }
    }
}