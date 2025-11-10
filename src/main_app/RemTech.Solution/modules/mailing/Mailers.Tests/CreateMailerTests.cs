using Mailers.Persistence.NpgSql;
using RemTech.Aes.Encryption;

namespace Mailers.Tests;

public sealed class CreateMailerTests(MailersTestsServices services) : IClassFixture<MailersTestsServices>
{
    [Fact]
    private async Task Create_Mailer_Success()
    {
        string email = "mail@email.com";
        string password = "sdaddsa-dsadsads-dasdas";
        await using var scope = services.Scope();
        await using var session = scope.ServiceProvider.GetRequiredService<NpgSqlSession>();
        var createMailer = scope.ServiceProvider
            .GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments(email, password);
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.True(mailer.IsSuccess);

        var insertMailer = scope.ServiceProvider
            .GetRequiredService<IAsyncFunction<InsertMailerFunctionArgument, Result<Unit>>>();
        var insertArgument = new InsertMailerFunctionArgument(session, mailer);
        var insertion = await insertMailer.Invoke(insertArgument, CancellationToken.None);
        Assert.True(insertion.IsSuccess);
    }

    [Fact]
    private async Task Create_Mailer_Duplicate_Email_Failure()
    {
        string email = "mail@email.com";
        string password = "sdaddsa-dsadsads-dasdas";
        await using var scope = services.Scope();
        await using var session = scope.ServiceProvider.GetRequiredService<NpgSqlSession>();
        var createMailer = scope.ServiceProvider
            .GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments(email, password);
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.True(mailer.IsSuccess);

        var insertMailer = scope.ServiceProvider
            .GetRequiredService<IAsyncFunction<InsertMailerFunctionArgument, Result<Unit>>>();
        var insertArgument = new InsertMailerFunctionArgument(session, mailer);
        await insertMailer.Invoke(insertArgument, CancellationToken.None);
        var insertion = await insertMailer.Invoke(insertArgument, CancellationToken.None);
        Assert.True(insertion.IsFailure);
    }

    [Fact]
    private async Task Register_Mailer_In_Application_Success()
    {
        string email = "mail@email.com";
        string password = "sdaddsa-dsadsads-dasdas";
        await using var scope = services.Scope();
        await using var session = scope.ServiceProvider.GetRequiredService<NpgSqlSession>();
        var registration = scope.ServiceProvider
            .GetRequiredService<IAsyncFunction<RegisterMailerInApplicationFunctionArgument, Result<Mailer>>>();
        var input = new RegisterMailerInApplicationFunctionArgument(email, password, session);
        var registered = await registration.Invoke(input, CancellationToken.None);
        Assert.True(registered.IsSuccess);
        string hashedSmtp = registered.Value.Metadata.Password.Value;
        Assert.NotEqual(password, hashedSmtp);

        await using (var scope1 = services.Scope())
        {
            await using var session1 = scope.ServiceProvider.GetRequiredService<NpgSqlSession>();
            var query = new QueryMailerArguments(Id: registered.Value.Metadata.Id);
            var inserted = await query.Get(session, CancellationToken.None);
            Assert.True(inserted.HasValue);
            Assert.Equal(hashedSmtp, inserted.Value.Metadata.Password.Value);
        }
    }

    [Fact]
    private void Create_Invalid_Email()
    {
        string email = "invalid-email";
        var scope = services.Scope();
        var createEmail = scope.ServiceProvider
            .GetRequiredService<IFunction<CreateEmailFunctionArgument, Result<Email>>>();
        var result = createEmail.Invoke(new CreateEmailFunctionArgument(email));
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Create_Mailer_Invalid_Email()
    {
        await using var scope = services.Scope();
        var createMailer = scope.ServiceProvider
            .GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("invalid-email", "sdaddsa-dsadsads-dasdas");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.False(mailer.IsSuccess);
    }

    [Fact]
    private async Task Create_Mailer_Empty_Email()
    {
        await using var scope = services.Scope();
        var createMailer = scope.ServiceProvider
            .GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("", "sdaddsa-dsadsads-dasdas");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.False(mailer.IsSuccess);
    }

    [Fact]
    private async Task Create_Mailer_Empty_SmtpPassword()
    {
        await using var scope = services.Scope();
        var createMailer = scope.ServiceProvider
            .GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("test@email.com", "");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        Assert.False(mailer.IsSuccess);
    }

    [Fact]
    private async Task Encrypt_String_Tests()
    {
        AesEncryption encryption = new("1234567890123456");
        string plainText = "Hello world string.";
        string encryptedText = await encryption.EncryptAsync(plainText);
        Assert.NotEqual(plainText, encryptedText);
    }

    [Fact]
    private async Task Decrypt_String_Tests()
    {
        AesEncryption encryption = new("1234567890123456");
        string plainText = "Hello world string.";
        string encryptedText = await encryption.EncryptAsync(plainText);
        string decryptedText = await encryption.DecryptAsync(encryptedText);
        Assert.Equal(plainText, decryptedText);
    }

    [Fact]
    private async Task Decrypt_String_Tests_Failure()
    {
        AesEncryption encryption = new("1234567890123456");
        string plainText = "Hello world string.";
        string encryptedFake = "Not a hello world string.";
        string decryptedText = await encryption.DecryptAsync(encryptedFake);
        Assert.NotEqual(plainText, decryptedText);
    }

    [Fact]
    private async Task Decrypt_String_Tests_Invalid_Format_Failure()
    {
        AesEncryption encryption = new("1234567890123456");
        string plainText = "Hello world string.";
        string encryptedFake = Guid.NewGuid().ToString();
        string decryptedText = await encryption.DecryptAsync(encryptedFake);
        Assert.NotEqual(plainText, decryptedText);
    }
}