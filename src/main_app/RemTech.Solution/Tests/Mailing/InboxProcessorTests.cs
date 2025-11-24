using Mailing.Presenters.Mailers.AddMailer;
using Tests.ModuleFixtures;

namespace Tests.Mailing;

public sealed class InboxProcessorTests(CompositionRootFixture fixture) : IClassFixture<CompositionRootFixture>
{
    private readonly MailingModule _module = fixture.MailingModule;
    
    [Fact]
    private async Task Test_Inbox_Processor_Works_Success()
    {
        string smtpPassword = "test smpt password";
        string email = "testEmail@gmail.com";
        Result<AddMailerResponse> mailer = await _module.AddMailer(smtpPassword, email);
        Assert.True(mailer.IsSuccess);
        await _module.SeedInboxMessages();
        Assert.True(await _module.HasInboxMessages()); // expects some messages in database
        await Task.Delay(TimeSpan.FromSeconds(30)); // delay, so processor can remove all pending messages.
        Assert.False(await _module.HasInboxMessages()); // expects no messages in database
    }
}