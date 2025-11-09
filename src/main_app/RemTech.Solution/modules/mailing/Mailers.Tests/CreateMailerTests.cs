namespace Mailers.Tests;

public sealed class CreateMailerTests(MailersTestsServices services) : IClassFixture<MailersTestsServices>
{
    [Fact]
    private async Task Create_Mailer_Success()
    {
        await using var scope = services.Scope();
        await using var session = scope.ServiceProvider.GetRequiredService<NpgSqlSession>();
        var createMailer = scope.ServiceProvider.GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("mail@email.com", "sdaddsa-dsadsads-dasdas");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.True(mailer.IsSuccess);
        
        var insertMailer = scope.ServiceProvider.GetRequiredService<IAsyncFunction<InsertMailerFunctionArgument, Result<Unit>>>();
        var insertArgument = new InsertMailerFunctionArgument(session, mailer);
        var insertion = await insertMailer.Invoke(insertArgument, CancellationToken.None);
        Assert.True(insertion.IsSuccess);
    }

    [Fact]
    private async Task Create_Mailer_Duplicate_Email_Failure()
    {
        await using var scope = services.Scope();
        await using var session = scope.ServiceProvider.GetRequiredService<NpgSqlSession>();
        var createMailer = scope.ServiceProvider.GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("mail@email.com", "sdaddsa-dsadsads-dasdas");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.True(mailer.IsSuccess);
        
        var insertMailer = scope.ServiceProvider.GetRequiredService<IAsyncFunction<InsertMailerFunctionArgument, Result<Unit>>>();
        var insertArgument = new InsertMailerFunctionArgument(session, mailer);
        await insertMailer.Invoke(insertArgument, CancellationToken.None);
        var insertion = await insertMailer.Invoke(insertArgument, CancellationToken.None);
        Assert.True(insertion.IsFailure);
    }

    [Fact]
    private async Task Register_Mailer_In_Application_Success()
    {
        await using var scope = services.Scope();
        await using var session = scope.ServiceProvider.GetRequiredService<NpgSqlSession>();
        var registration = scope.ServiceProvider.GetRequiredService<IAsyncFunction<RegisterMailerInApplicationFunctionArgument, Result<Mailer>>>();
        var input = new RegisterMailerInApplicationFunctionArgument("mail@email.com", "sdssad-dsadsadsa-dsadsasda", session);
        var registered = await registration.Invoke(input, CancellationToken.None);
        Assert.True(registered.IsSuccess);
    }
    
    [Fact]
    private void Create_Invalid_Email()
    {
        string email = "invalid-email";
        var scope = services.Scope();
        var createEmail = scope.ServiceProvider.GetRequiredService<IFunction<CreateEmailFunctionArgument, Result<Email>>>();
        var result = createEmail.Invoke(new CreateEmailFunctionArgument(email));
        Assert.True(result.IsFailure);
    }
    
    [Fact]
    private async Task Create_Mailer_Invalid_Email()
    {
        await using var scope = services.Scope();
        var createMailer = scope.ServiceProvider.GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("invalid-email", "sdaddsa-dsadsads-dasdas");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.False(mailer.IsSuccess);
    }

    [Fact]
    private async Task Create_Mailer_Empty_Email()
    {
        await using var scope = services.Scope();
        var createMailer = scope.ServiceProvider.GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("", "sdaddsa-dsadsads-dasdas");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.False(mailer.IsSuccess);
    }

    [Fact]
    private async Task Create_Mailer_Empty_SmtpPassword()
    {
        await using var scope = services.Scope();
        var createMailer = scope.ServiceProvider.GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("test@email.com", "");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.False(mailer.IsSuccess);
    }
}