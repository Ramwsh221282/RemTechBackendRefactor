using Mailing.Presenters.Mailers.AddMailer;
using Tests.ModuleFixtures;

namespace Tests.Mailing;

public sealed class CreateMailerTests(CompositionRootFixture services) : IClassFixture<CompositionRootFixture>
{
    private const string email = "testMail@gmail.com";
    private const string password = "sdaddsa-dsadsads-dasdas";
    private readonly MailingModule _module = services.MailingModule;

    [Fact]
    private async Task Create_Mailer_Success()
    {
        Result<AddMailerResponse> result = await _module.AddMailer(password, email);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Create_Mailer_Duplicate_Email_Failure()
    {
        await _module.AddMailer(password, email);
        Result<AddMailerResponse> result = await _module.AddMailer(password, email);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Create_Mailer_Invalid_Email()
    {
        const string invalidEmail = "some invalid email";
        Result<AddMailerResponse> result = await _module.AddMailer(password, invalidEmail);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Create_Mailer_Invalid_Password()
    {
        const string invalidPassword = "   ";
        await _module.AddMailer(password, email);
        Result<AddMailerResponse> result = await _module.AddMailer(invalidPassword, email);
        Assert.True(result.IsFailure);
    }
}