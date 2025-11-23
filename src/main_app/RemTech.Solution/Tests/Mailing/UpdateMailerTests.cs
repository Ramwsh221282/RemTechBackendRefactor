using Mailing.Presenters.Mailers.AddMailer;
using Mailing.Presenters.Mailers.UpdateMailer;
using Tests.ModuleFixtures;

namespace Tests.Mailing;

public sealed class UpdateMailerTests(CompositionRootFixture fixture) : IClassFixture<CompositionRootFixture>
{
    private readonly MailingModule _module = fixture.MailingModule;

    [Fact]
    private async Task Update_Mailer_Success()
    {
        string oldEmail = "testMail@gmail.com";
        string oldPassword = "tedas-dsadas-dsadasddsa";
        Result<AddMailerResponse> created = await _module.AddMailer(oldPassword, oldEmail);
        Assert.True(created.IsSuccess);
        string newEmail = "newEmail@mail.com";
        string newPassword = "dasdasdasdas-dasdasdas-dasdasasd";
        Result<UpdateMailerResponse> updated = await _module.UpdateMailer(created.Value.MailerId, newPassword, newEmail);
        Assert.True(updated.IsSuccess);
    }

    [Fact]
    private async Task Update_Mailer_Not_Found_Failure()
    {
        string newEmail = "newEmail@mail.com";
        string newPassword = "dasdasdasdas-dasdasdas-dasdasasd";
        Result<UpdateMailerResponse> updated = await _module.UpdateMailer(Guid.NewGuid(), newPassword, newEmail);
        Assert.True(updated.IsFailure);
    }
    
    [Fact]
    private async Task Update_Mailer_Not_Full_Credentials_Failure()
    {
        string oldEmail = "testMail@gmail.com";
        string oldPassword = "";
        Result<AddMailerResponse> created = await _module.AddMailer(oldPassword, oldEmail);
        Assert.True(created.IsSuccess);
        string newEmail = "newEmail@mail.com";
        Result<UpdateMailerResponse> updated = await _module.UpdateMailer(created.Value.MailerId, null!, newEmail);
        Assert.True(updated.IsFailure);
    }
}