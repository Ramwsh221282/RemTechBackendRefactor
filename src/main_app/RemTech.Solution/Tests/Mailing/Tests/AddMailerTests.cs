using Mailing.Presenters.Mailers.AddMailer;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.Mailing.Features;

namespace Tests.Mailing.Tests;

public sealed class AddMailerTests(MailingModuleFixture fixture) : IClassFixture<MailingModuleFixture>
{
    private readonly MailingModuleFeaturesFacade _facade = new(fixture.Services);

    [Fact]
    private async Task Add_Mailer_Success()
    {
        const string email = "myEmail@gmail.com";
        const string password = "some smtp password";
        Result<AddMailerResponse> result = await _facade.AddMailer(email, password);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("not valid email")]
    [InlineData("   ")]
    private async Task Add_Mailer_Invalid_Email(string email)
    {
        const string password = "some smtp password";
        Result<AddMailerResponse> result = await _facade.AddMailer(email, password);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Add_Mailer_Invalid_Password()
    {
        const string email = "myEmail@gmail.com";
        const string password = "  ";
        Result<AddMailerResponse> result = await _facade.AddMailer(email, password);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Add_Mailer_Email_Duplicate()
    {
        const string email = "myEmail@gmail.com";
        const string password = "some smtp password";
        await _facade.AddMailer(email, password);
        Result<AddMailerResponse> result = await _facade.AddMailer(email, password);
        Assert.True(result.IsFailure);
    }
}