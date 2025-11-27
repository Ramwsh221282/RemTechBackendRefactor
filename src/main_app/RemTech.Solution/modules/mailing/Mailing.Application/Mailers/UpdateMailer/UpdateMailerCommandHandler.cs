using Mailing.Core.Common;
using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace Mailing.Application.Mailers.UpdateMailer;

public sealed class UpdateMailerCommandHandler (
    GetMailerProtocol getProtocol,
    SaveMailerProtocol saveProtocol,
    EncryptMailerSmtpPasswordProtocol encryptProtocol)
    : ICommandHandler<UpdateMailerCommand, Mailer>
{
    public async Task<Mailer> Execute(UpdateMailerCommand args)
    {
        Mailer? mailer = await Mailer.GetById(args.Id, getProtocol, args.Ct, withLock: true);
        if (mailer == null) 
            throw ErrorException.NotFound("Конфигурация почтового сервиса не найдена.");
        
        Email email = new(args.NewEmail);
        MailerConfig config = new(args.NewPassword);
        
        Mailer updated = mailer 
            with { Domain = mailer.Domain with { Email = email } } 
            with { Config = config };
        
        updated.Domain.Validate();
        updated.Config.Validate();
        
        Mailer encrypted = await updated.Encrypted(encryptProtocol, args.Ct);
        await encrypted.Save(saveProtocol, args.Ct);
        return encrypted;
    }
}