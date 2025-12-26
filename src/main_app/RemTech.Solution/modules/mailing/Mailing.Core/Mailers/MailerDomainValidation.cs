using Mailing.Core.Common;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace Mailing.Core.Mailers;

public static class MailerDomainValidation
{
    extension(MailerDomain domain)
    {
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(domain.Service))
                throw ErrorException.ValueNotSet("Название сервиса отправки email");
            if (string.IsNullOrWhiteSpace(domain.SmtpHost))
                throw ErrorException.ValueNotSet("SMTP сервис отправки email");
            if (domain.SendLimit < 0)
                throw ErrorException.ValueInvalid("Лимит отправленных сообщений", domain.SendLimit.ToString());
            if (domain.CurrentSend < 0)
                throw ErrorException.ValueInvalid("Текущее число отправленных сообщений", domain.CurrentSend.ToString());
            domain.Email.Validate();
        }
    }
}