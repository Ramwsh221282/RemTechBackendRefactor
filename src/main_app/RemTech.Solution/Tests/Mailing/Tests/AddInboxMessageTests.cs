using Mailing.Presenters.Inbox.CreateInboxMessage;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.Mailing.Features;

namespace Tests.Mailing.Tests;

public sealed class AddInboxMessageTests(MailingModuleFixture fixture) : IClassFixture<MailingModuleFixture>
{
    private readonly MailingModuleFeaturesFacade _facade = new(fixture.Services);
    
    [Fact]
    private async Task Add_Inbox_Message_Success()
    {
        const string targetEmail = "myEmail@gmail.com";
        const string subject = "test subject";
        const string body = "test body";
        Result<CreateInboxMessageResponse> result = await _facade.CreateInboxMessage(targetEmail, subject, body);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("   ")]
    [InlineData(" not even email  ")]
    private async Task Add_Inbox_Message_Invalid_Email_Failure(string targetEmail)
    {
        const string subject = "test subject";
        const string body = "test body";
        Result<CreateInboxMessageResponse> result = await _facade.CreateInboxMessage(targetEmail, subject, body);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Add_Inbox_Message_Invalid_Subject_Failure()
    {
        const string targetEmail = "myEmail@gmail.com";
        const string subject = "  ";
        const string body = "test body";
        Result<CreateInboxMessageResponse> result = await _facade.CreateInboxMessage(targetEmail, subject, body);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Add_Inbox_Message_Invalid_Body_Failure()
    {
        const string targetEmail = "myEmail@gmail.com";
        const string subject = "test subject";
        const string body = "  ";
        Result<CreateInboxMessageResponse> result = await _facade.CreateInboxMessage(targetEmail, subject, body);
        Assert.True(result.IsFailure);
    }
}