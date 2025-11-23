using Mailing.Application.Mailers.SendEmail;
using Mailing.Core.Common;
using Mailing.Core.Inbox;
using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using Mailing.Presenters.Mailers.TestEmailSending;

namespace Tests.Mailing;

public sealed class MailSendingTests
{
    [Fact]
    private async Task Send_Email_When_Mailer_Excees_Limit_Failure()
    {
        Email email = new("test email");
        MailerDomain domain = new(email, "test service", "test-host", 300, 300);
        MailerConfig config = new("test password");
        Mailer mailer = new(Guid.NewGuid(), domain, config);

        FakeGetMailerProtocol getMailer = new(mailer);
        FakeDecryptProtocol decrypt = new();
        FakeEncryptProtocol encrypt = new();
        FakeSaveMailerProtocol saveMailer = new();
        FakeDeliveryProtocol delivery = new();
        SendEmailCommandHandler handler = new(getMailer, decrypt, encrypt, saveMailer, delivery);
        TestEmailSendingGateway gateway = new(handler);
        
        Result<TestEmailSendingResponse> result 
            = await gateway.Execute(new TestEmailSendingRequest("test email", mailer.Id, CancellationToken.None));
        
        Assert.True(result.IsFailure);
    }

    private class FakeDeliveryProtocol : MessageDeliveryProtocol
    {
        public async Task<DeliveredMessage> Process(Mailer mailer, InboxMessage message, CancellationToken ct)
        {
            await Task.Yield();
            return new DeliveredMessage(mailer, message);
        }
    }
    
    private class FakeSaveMailerProtocol : SaveMailerProtocol
    {
        public async Task Save(Mailer mailer, CancellationToken ct)
        {
            await Task.Yield();
        }
    }
    
    private class FakeEncryptProtocol : EncryptMailerSmtpPasswordProtocol
    {
        public async Task<Mailer> WithEncryptedPassword(Mailer mailer, CancellationToken ct)
        {
            await Task.Yield();
            return mailer;
        }
    }
    
    private class FakeDecryptProtocol : DecryptMailerSmtpPasswordProtocol
    {
        public async Task<Mailer> WithDecryptedPassword(Mailer mailer, CancellationToken ct)
        {
            await Task.Yield();
            return mailer;
        }
    }
    
    private class FakeGetMailerProtocol(Mailer mailer) : GetMailerProtocol
    {
        public async Task<Mailer?> AvailableBySendLimit(CancellationToken ct = default)
        {
            await Task.Yield();
            return mailer;
        }

        public async Task<Mailer?> ByEmail(string email, CancellationToken ct = default)
        {
            await Task.Yield();
            return mailer;
        }

        public async Task<Mailer?> ById(Guid id, CancellationToken ct = default)
        {
            await Task.Yield();
            return mailer;
        }
    }
}