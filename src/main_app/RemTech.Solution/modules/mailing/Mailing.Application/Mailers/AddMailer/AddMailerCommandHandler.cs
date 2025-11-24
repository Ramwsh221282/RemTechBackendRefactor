using Mailing.Core.Common;
using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;
using Serilog;

namespace Mailing.Application.Mailers.AddMailer;

public sealed class AddMailerCommandHandler 
(
    ILogger logger,
    PersistMailerProtocol persistMailer,
    GetMailerProtocol getProtocol,
    EncryptMailerSmtpPasswordProtocol encryptPassword)
    : ICommandHandler<AddMailerCommand, Mailer>
{
    public async Task<Mailer> Execute(AddMailerCommand args)
    {
        logger.Information("Creating new mailer.");
        Email email = new(Value: args.Email);
        MailerDomain domain = new(Service: "", Email: email, SendLimit: 0, SmtpHost: "", CurrentSend: 0);
        MailerDomain resolved = domain.WithResolvedService();
        MailerConfig config = new(SmtpPassword: args.Password);
        Mailer mailer = new(Id: Guid.NewGuid(), Domain: resolved, Config: config);
        Mailer hashed = await mailer.Encrypted(encryptPassword, args.Ct);
        Mailer valid = hashed.Validated();

        Mailer? existingByEmail = await Mailer.GetByEmail(email.Value, getProtocol, args.Ct);
        if (existingByEmail != null) throw ErrorException.Conflict($"Почтовый сервис с почтой {email.Value} уже существует");
        
        await valid.Persist(persistMailer, args.Ct);
        logger.Information("New mailer created.");
        return valid;
    }
}