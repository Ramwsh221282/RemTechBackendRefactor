using Mailing.Core.Common;
using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace Mailing.Application.Mailers.UpdateMailer;

public sealed class UpdateMailerCommandHandler (
    GetMailerProtocol getProtocol,
    SaveMailerProtocol saveProtocol,
    EnsureMailerEmailUniqueProtocol emailUniqueProtocol,
    EncryptMailerSmtpPasswordProtocol encryptProtocol)
    : ICommandHandler<UpdateMailerCommand, Mailer>
{
    public async Task<Mailer> Execute(UpdateMailerCommand args)
    {
        CancellationToken ct = args.Ct;
        Mailer? mailer = await Mailer.GetById(args.Id, getProtocol, ct, withLock: true);
        if (mailer == null) 
            throw ErrorException.NotFound("Конфигурация почтового сервиса не найдена.");
        
        Email email = new(args.NewEmail);
        MailerConfig config = new(args.NewPassword);
        
        Mailer updated = mailer 
            with { Domain = mailer.Domain with { Email = email } } 
            with { Config = config };
        
        updated.Domain.Validate();
        updated.Config.Validate();
        await emailUniqueProtocol.EnsureEmailUnique(updated, ct);
        
        Mailer encrypted = await updated.Encrypted(encryptProtocol, ct);
        await encrypted.Save(saveProtocol, ct);
        return encrypted;
    }
}