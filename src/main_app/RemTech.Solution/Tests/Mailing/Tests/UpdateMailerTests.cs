using Mailing.Presenters.Mailers.AddMailer;
using Mailing.Presenters.Mailers.UpdateMailer;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.Mailing.Features;

namespace Tests.Mailing.Tests;

public sealed class UpdateMailerTests(MailingModuleFixture fixture) : IClassFixture<MailingModuleFixture>
{
    private readonly MailingModuleFeaturesFacade _facade = new(fixture.Services);

    [Fact]
    private async Task Update_Mailer_Success()
    {
        const string defaultEmail = "defaultEmail@gmail.com";
        const string defaultPassword = "defaultPassword";
        const string newEmail = "newEmail@gmail.com";
        const string newPassword = "newPassword";
        Result<AddMailerResponse> mailer = await _facade.AddMailer(defaultEmail, defaultPassword);
        Assert.True(mailer.IsSuccess);
        Guid id = mailer.Value.MailerId;
        Result<UpdateMailerResponse> update = await _facade.UpdateMailer(id, newEmail, newPassword);
        Assert.True(update.IsSuccess);
        bool emailEquals = await _facade.MailerEmailEquals(id, newEmail);
        bool passwordEquals = await _facade.MailerPasswordEquals(id, newPassword);
        Assert.True(emailEquals);
        Assert.True(passwordEquals);
    }

    [Fact]
    private async Task Update_Mailer_Duplicate_Email_Failure()
    {
        const string defaultEmail = "defaultEmail@gmail.com";
        const string defaultPassword = "defaultPassword";
        const string newPassword = "newPassword";
        Result<AddMailerResponse> mailer = await _facade.AddMailer(defaultEmail, defaultPassword);
        Assert.True(mailer.IsSuccess);
        Guid id = mailer.Value.MailerId;
        Result<UpdateMailerResponse> update = await _facade.UpdateMailer(id, defaultEmail, newPassword);
        Assert.True(update.IsFailure);
    }
    
    [Fact]
    private async Task Update_Mailer_Invalid_Email_And_Password()
    {
        const string defaultEmail = "defaultEmail@gmail.com";
        const string defaultPassword = "defaultPassword";
        const string newPassword = "  ";
        const string newEmail = "not valid email";
        Result<AddMailerResponse> mailer = await _facade.AddMailer(defaultEmail, defaultPassword);
        Assert.True(mailer.IsSuccess);
        Guid id = mailer.Value.MailerId;
        Result<UpdateMailerResponse> update = await _facade.UpdateMailer(id, newEmail, newPassword);
        Assert.True(update.IsFailure);
    }

    [Fact]
    private async Task Update_Mailer_Empty_Email_And_Invalid_Password()
    {
        const string defaultEmail = "defaultEmail@gmail.com";
        const string defaultPassword = "defaultPassword";
        const string newPassword = "  ";
        const string newEmail = "  ";
        Result<AddMailerResponse> mailer = await _facade.AddMailer(defaultEmail, defaultPassword);
        Assert.True(mailer.IsSuccess);
        Guid id = mailer.Value.MailerId;
        Result<UpdateMailerResponse> update = await _facade.UpdateMailer(id, newEmail, newPassword);
        Assert.True(update.IsFailure);
    }
    
    [Fact]
    private async Task Update_Mailer_Invalid_Email()
    {
        const string defaultEmail = "defaultEmail@gmail.com";
        const string defaultPassword = "defaultPassword";
        const string newPassword = "newPassword";
        const string newEmail = "not valid email";
        Result<AddMailerResponse> mailer = await _facade.AddMailer(defaultEmail, defaultPassword);
        Assert.True(mailer.IsSuccess);
        Guid id = mailer.Value.MailerId;
        Result<UpdateMailerResponse> update = await _facade.UpdateMailer(id, newEmail, newPassword);
        Assert.True(update.IsFailure);
    }
    
    [Fact]
    private async Task Update_Mailer_Empty_Email()
    {
        const string defaultEmail = "defaultEmail@gmail.com";
        const string defaultPassword = "defaultPassword";
        const string newPassword = "newPassword";
        const string newEmail = "  ";
        Result<AddMailerResponse> mailer = await _facade.AddMailer(defaultEmail, defaultPassword);
        Assert.True(mailer.IsSuccess);
        Guid id = mailer.Value.MailerId;
        Result<UpdateMailerResponse> update = await _facade.UpdateMailer(id, newEmail, newPassword);
        Assert.True(update.IsFailure);
    }
}