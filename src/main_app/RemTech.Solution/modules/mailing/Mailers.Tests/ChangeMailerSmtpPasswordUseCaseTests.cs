using Mailers.Application.Configs;
using Mailers.Application.Features.ChangeMailerSmtpPassword;
using Mailers.Application.Features.CreateMailer;
using Mailers.Application.Features.Encryptions;
using Mailers.Core.MailersModule;
using Microsoft.Extensions.Options;

namespace Mailers.Tests;

public sealed class ChangeMailerSmtpPasswordUseCaseTests(MailersTestsServices services) : IClassFixture<MailersTestsServices>
{
    private const string email = "test@mail.com";
    private const string passwordBeforeChange = "test-smtp-password";
    private static readonly CancellationToken ct = CancellationToken.None;
    
    [Fact]
    private async Task Change_Mailer_Smtp_Password_Success()
    {
        string passwordAfterChange = "test-other-smtp-password";
        
        await using AsyncServiceScope scope = services.Scope();
        IOptions<MailersEncryptOptions> encrypt = scope.ServiceProvider.GetRequiredService<IOptions<MailersEncryptOptions>>();
        CreateMailerUseCase createMailer = scope.ServiceProvider.GetRequiredService<CreateMailerUseCase>();
        ChangeMailerSmtpPasswordUseCase changePassword = scope.ServiceProvider.GetRequiredService<ChangeMailerSmtpPasswordUseCase>();

        CreateMailerArgs createArgs = new(email, passwordBeforeChange, ct);
        Result<Mailer> created = await createMailer.Invoke(createArgs);
        Assert.True(created.IsSuccess);

        ChangeMailerSmtpPasswordArgs changeArgs = new(created.Value.Metadata.Id.Value, passwordAfterChange, ct);
        Result<Mailer> changed = await changePassword.Invoke(changeArgs);
        Assert.True(changed.IsSuccess);

        Result<string> decrypted = await encrypt.Decrypted(changed);
        Assert.NotEqual(passwordBeforeChange, decrypted.Value);
        Assert.Equal(passwordAfterChange, decrypted.Value);
    }
    
    [Fact]
    private async Task Change_Mailer_Smtp_Password_Invalid_Password_Failure()
    {
        const string passwordAfterChange = "  ";
        
        await using AsyncServiceScope scope = services.Scope();
        CreateMailerUseCase createMailer = scope.ServiceProvider.GetRequiredService<CreateMailerUseCase>();
        ChangeMailerSmtpPasswordUseCase changePassword = scope.ServiceProvider.GetRequiredService<ChangeMailerSmtpPasswordUseCase>();

        CreateMailerArgs createArgs = new(email, passwordBeforeChange, ct);
        Result<Mailer> created = await createMailer.Invoke(createArgs);
        Assert.True(created.IsSuccess);

        ChangeMailerSmtpPasswordArgs changeArgs = new(created.Value.Metadata.Id.Value, passwordAfterChange, ct);
        Result<Mailer> changed = await changePassword.Invoke(changeArgs);
        Assert.True(changed.IsFailure);
    }

    [Fact]
    private async Task Change_Mailer_Smtp_Password_Mailer_Not_Found_Failure()
    {
        const string passwordAfterChange = "test-other-smtp-password";
        
        await using AsyncServiceScope scope = services.Scope();
        CreateMailerUseCase createMailer = scope.ServiceProvider.GetRequiredService<CreateMailerUseCase>();
        ChangeMailerSmtpPasswordUseCase changePassword = scope.ServiceProvider.GetRequiredService<ChangeMailerSmtpPasswordUseCase>();

        CreateMailerArgs createArgs = new(email, passwordBeforeChange, ct);
        Result<Mailer> created = await createMailer.Invoke(createArgs);
        Assert.True(created.IsSuccess);

        ChangeMailerSmtpPasswordArgs changeArgs = new(Guid.NewGuid(), passwordAfterChange, ct);
        Result<Mailer> changed = await changePassword.Invoke(changeArgs);
        Assert.True(changed.IsFailure);
    }
}