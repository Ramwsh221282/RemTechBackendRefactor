using Mailing.Application.Mailers.SendEmail;
using Mailing.Core.Common;
using Mailing.Core.Inbox;
using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using Mailing.Presenters.Mailers.AddMailer;
using Mailing.Presenters.Mailers.TestEmailSending;
using Tests.ModuleFixtures;

namespace Tests.Mailing;

public sealed class MailSendingTests(CompositionRootFixture fixture) : IClassFixture<CompositionRootFixture>
{
    private readonly MailingModule _module = fixture.MailingModule;
    
    [Fact]
    private async Task Send_Email_Success()
    {
        string email = "jimkrauz@gmail.com";
        string smtpPassword = "dasdsa dsadas dasdas dsadas";
        Result<AddMailerResponse> creatingMailer = await _module.AddMailer(smtpPassword, email);
        Assert.True(creatingMailer.IsSuccess);
        Result<TestEmailSendingResponse> result = await _module.TestEmailSending(email, creatingMailer.Value.MailerId);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Send_Email_Twice_Success()
    {
        // tests impact of second time encryption and decryption when sending email.
        // because every time mailer gets decrypted and then encrypted, the mailer has other random smtp key.
        string email = "jimkrauz@gmail.com";
        string smtpPassword = "lsav arhl dzad rfzd";
        Result<AddMailerResponse> creatingMailer = await _module.AddMailer(smtpPassword, email);
        Assert.True(creatingMailer.IsSuccess);
        Result<TestEmailSendingResponse> result1 = await _module.TestEmailSending(email, creatingMailer.Value.MailerId);
        Result<TestEmailSendingResponse> result2 = await _module.TestEmailSending(email, creatingMailer.Value.MailerId);
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
    }
    
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
        public async Task<Mailer?> AvailableBySendLimit(bool withLock = false, CancellationToken ct = default)
        {
            await Task.Yield();
            return mailer;
        }

        public async Task<Mailer?> Get(GetMailerQueryArgs args, CancellationToken ct = default)
        {
            await Task.Yield();
            return mailer;
        }
    }
}