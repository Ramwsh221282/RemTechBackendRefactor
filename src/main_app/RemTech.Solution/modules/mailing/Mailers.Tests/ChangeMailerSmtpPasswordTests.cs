namespace Mailers.Tests;

public sealed class ChangeMailerSmtpPasswordTests(MailersTestsServices services) : IClassFixture<MailersTestsServices>
{
    [Fact]
    private async Task Change_Mailer_Smtp_Password_Success()
    {
        string email = "test@mail.com";
        string passwordBeforeChange = "test-smtp-password";
        string passwordAfterChange = "test-other-smtp-password";
        CancellationToken ct = CancellationToken.None;
        await using AsyncServiceScope scope = services.Scope();
        NpgSqlSession session = scope.ServiceProvider.GetRequiredService<NpgSqlSession>();
        
        IAsyncFunction<RegisterMailerInApplicationFunctionArgument, Result<Mailer>> registerMailer =
            scope.ServiceProvider.GetRequiredService<IAsyncFunction<RegisterMailerInApplicationFunctionArgument, Result<Mailer>>>();
        
        RegisterMailerInApplicationFunctionArgument registerArg = new(email, passwordBeforeChange, session);
        Result<Mailer> created = await registerMailer.Invoke(registerArg, ct);
        Assert.True(created.IsSuccess);

        await using (AsyncServiceScope scope1 = services.Scope())
        {
            await using NpgSqlSession session1 = scope1.ServiceProvider.GetRequiredService<NpgSqlSession>();
            
            IAsyncFunction<ChangeMailerSmtpPasswordFunctionArguments, Result<Mailer>> changeSmtp =
                scope1.ServiceProvider.GetRequiredService<IAsyncFunction<ChangeMailerSmtpPasswordFunctionArguments, Result<Mailer>>>();

            ChangeMailerSmtpPasswordFunctionArguments changeArg = new(created.Value.Metadata.Id, passwordAfterChange, session1);
            Result<Mailer> created2 = await changeSmtp.Invoke(changeArg, ct);
            Assert.True(created2.IsSuccess);

            string smtpBeforeChange = created.Value.Metadata.Password.Value;
            string smtpAfterChange = created2.Value.Metadata.Password.Value;
            Assert.NotEqual(smtpBeforeChange, smtpAfterChange);
        }
    }
}