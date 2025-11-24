using Mailing.Core.Common;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace Mailing.Core.Mailers;

public sealed class MailerDomainUpdateShell(string? newEmail, string? newSmtpPassword)
{
    public Mailer Update(Mailer mailer)
    {
        if (string.IsNullOrWhiteSpace(newEmail) || string.IsNullOrWhiteSpace(newSmtpPassword))
            throw ErrorException.Conflict("Невозможно обновить конфигурацию почтового сервиса. Обязательно требуется новый email и SMTP ключ.");
        
        Email updatedEmail = new(newEmail);
        MailerConfig newConfig = new(newSmtpPassword);
        MailerDomain updatedDomain = mailer.Domain with { Email = updatedEmail };
        return mailer with { Domain = updatedDomain.WithResolvedService(), Config = newConfig };
    }
}