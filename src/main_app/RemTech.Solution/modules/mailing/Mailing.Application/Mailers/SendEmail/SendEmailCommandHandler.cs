using System.Diagnostics;
using Mailing.Core.Common;
using Mailing.Core.Inbox;
using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace Mailing.Application.Mailers.SendEmail;

public sealed class SendEmailCommandHandler
(GetMailerProtocol getProtocol,
    DecryptMailerSmtpPasswordProtocol decryptProtocol,
    EncryptMailerSmtpPasswordProtocol encryptProtocol,
    SaveMailerProtocol saveMailer,
    MessageDeliveryProtocol deliveryProtocol)
    : ICommandHandler<SendEmailCommand, DeliveredMessage>
{
    public async Task<DeliveredMessage> Execute(SendEmailCommand args)
    {
        Mailer? mailer = await GetMailer(args);
        if (mailer == null) 
            throw ErrorException.NotFound("Не найден доступный почтовый сервис для отправки сообщения.");
        
        Email targetEmail = new(args.TargetEmail);
        MessageSubject subject = new(args.Subject);
        MessageBody body = new(args.Body);
        
        InboxMessage message = new InboxMessage(
                Id: Guid.NewGuid(), 
                TargetEmail: targetEmail, 
                Subject: subject, 
                Body: body)
            .Validated();
        
        Mailer decrypted = await mailer.Decrypted(decryptProtocol,  args.Ct);
        DeliveredMessage delivered = await decrypted.SendMessage(message, deliveryProtocol, args.Ct);
        Mailer encrypted = await delivered.Mailer.Encrypted(encryptProtocol, args.Ct);
        await encrypted.Save(saveMailer, args.Ct);
        return delivered;
    }

    private async Task<Mailer?> GetMailer(SendEmailCommand args)
    {
        Task<Mailer?> mailerReceiving = (args.SenderEmail, args.SenderId) switch
        {
            (null, not null) => Mailer.GetById(args.SenderId.Value, getProtocol, args.Ct, withLock: true),
            (not null, null) => Mailer.GetByEmail(args.SenderEmail, getProtocol, args.Ct, withLock: true),
            (null, null) => Mailer.GetByAvailableSendLimit(getProtocol, args.Ct, withLock: true),
            _ => throw new UnreachableException("Unable to resolve mailer receiving method.")
        };
        
        return await mailerReceiving;
    }
}