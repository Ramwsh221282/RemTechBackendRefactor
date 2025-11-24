using Mailing.Presenters.Inbox.CreateInboxMessage;
using Tests.ModuleFixtures;

namespace Tests.Mailing;

public sealed class CreateInboxMessageTests(CompositionRootFixture fixture) : IClassFixture<CompositionRootFixture>
{
    private const string targetEmail = "testEmail@gmail.com";
    private const string subject = "test Account";
    private const string testBody = "test Body";
    private readonly MailingModule _module = fixture.MailingModule;
    
    [Fact]
    private async Task Add_Inbox_Message_Success()
    {
        Result<CreateInboxMessageResponse> result = await _module.CreateInboxMessage(targetEmail, subject, testBody);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Add_Inbox_Message_Invalid_Email()
    {
        string invalidEmail = "not-valid-email";
        Result<CreateInboxMessageResponse> result = await _module.CreateInboxMessage(invalidEmail, subject, testBody);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Add_Inbox_Message_Invalid_Subject()
    {
        string invalidSubject = "    ";
        Result<CreateInboxMessageResponse> result = await _module.CreateInboxMessage(targetEmail, invalidSubject, testBody);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Add_Inbox_Message_Invalid_Body()
    {
        string invalidBody = "   ";
        Result<CreateInboxMessageResponse> result = await _module.CreateInboxMessage(targetEmail, subject, invalidBody);
        Assert.True(result.IsFailure);
    }
}