using Mailing.Core.Inbox;
using Mailing.Core.Mailers.Protocols;
using RemTech.Primitives.Extensions.Exceptions;

namespace Mailing.Core.Mailers;

public static class MailersManagement
{
    extension(Mailer)
    {
        public static async Task<Mailer?> GetById(
            Guid id, 
            GetMailerProtocol protocol, 
            CancellationToken ct)
        {
            return await protocol.ById(id, ct);
        }
        
        public static async Task<Mailer?> GetByEmail(
            string email, 
            GetMailerProtocol protocol, 
            CancellationToken ct)
        {
            return await protocol.ByEmail(email, ct);
        }

        public static async Task<Mailer?> GetByAvailableSendLimit(
            GetMailerProtocol protocol, 
            CancellationToken ct)
        {
            return await protocol.AvailableBySendLimit(ct);
        }
    }
    
    extension(Mailer mailer)
    {
        public async Task<DeliveredMessage> SendMessage(
            InboxMessage message, 
            MessageDeliveryProtocol protocol,   
            CancellationToken ct)
        {
            MailerDomain domain = mailer.Domain;
            if (domain.SendLimit == domain.CurrentSend) 
                throw ErrorException.Conflict("Почтовый сервис превысил лимит отправки сообщений. Нужно дождаться сброса.");
            return await protocol.Process(mailer, message, ct);
        }
        
        public async Task<Mailer> WithEncryptedSmtpPassword(
            EncryptMailerSmtpPasswordProtocol protocol, 
            CancellationToken ct)
        {
            return await protocol.WithEncryptedPassword(mailer, ct);
        }

        public async Task<Mailer> WithDecryptedSmtpPassword(
            DecryptMailerSmtpPasswordProtocol protocol,
            CancellationToken ct)
        {
            return await protocol.WithDecryptedPassword(mailer, ct);
        }

        public Mailer Update(MailerUpdateShell shell)
        {
            return shell.Update(mailer);
        }
        
        public async Task Persist(PersistMailerProtocol protocol, CancellationToken ct)
        {
            await protocol.Persist(mailer, ct);
        }

        public async Task Save(SaveMailerProtocol protocol, CancellationToken ct)
        {
            await protocol.Save(mailer, ct);
        }
    }
}