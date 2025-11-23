using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using RemTech.Primitives.Extensions.Exceptions;

namespace Mailing.Application.Mailers.UpdateMailer;

public sealed class UpdateMailerCommandHandler
(GetMailerProtocol getProtocol,
    SaveMailerProtocol saveProtocol,
    EncryptMailerSmtpPasswordProtocol encryptProtocol,
    DecryptMailerSmtpPasswordProtocol decryptProtocol)
    : ICommandHandler<UpdateMailerCommand, Mailer>
{
    public async Task<Mailer> Execute(UpdateMailerCommand args)
    {
        Mailer? mailer = await Mailer.GetById(args.Id, getProtocol, args.Ct);
        if (mailer == null) throw ErrorException.NotFound("Конфигурация почтового сервиса не найдена.");
        
        Mailer decrypted = await mailer.WithDecryptedSmtpPassword(decryptProtocol, args.Ct);
        MailerUpdateShell updateShell = new(args.NewEmail, args.NewPassword);
        Mailer updated = decrypted.Update(updateShell);
        Mailer encrypted = await updated.WithEncryptedSmtpPassword(encryptProtocol, args.Ct);
        await encrypted.Save(saveProtocol, args.Ct);
        return encrypted;
    }
}