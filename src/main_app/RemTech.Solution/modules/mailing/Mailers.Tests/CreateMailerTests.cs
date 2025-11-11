using Mailers.Application.Features.CreateMailer;
using Mailers.Core.MailersModule;

namespace Mailers.Tests;

public sealed class CreateMailerTests(MailersTestsServices services) : IClassFixture<MailersTestsServices>
{
    const string email = "mail@email.com";
    const string password = "sdaddsa-dsadsads-dasdas";
    private static readonly CancellationToken ct = CancellationToken.None;
    
    [Fact]
    private async Task Create_Mailer_Success()
    {
        await using AsyncServiceScope scope = services.Scope();
        CreateMailerUseCase createMailer = scope.ServiceProvider.GetRequiredService<CreateMailerUseCase>();
        CreateMailerArgs args = new(email, password, ct);
        Result<Mailer> created = await createMailer.Invoke(args);
        Assert.True(created.IsSuccess);
    }

    [Fact]
    private async Task Create_Mailer_Duplicate_Email_Failure()
    {
        await using (AsyncServiceScope scope = services.Scope())
        {
            CreateMailerUseCase createMailer = scope.ServiceProvider.GetRequiredService<CreateMailerUseCase>();
            CreateMailerArgs args = new(email, password, ct);
            Result<Mailer> created = await createMailer.Invoke(args);
            Assert.True(created.IsSuccess);
        }
        
        await using (AsyncServiceScope scope = services.Scope())
        {
            CreateMailerUseCase createMailer = scope.ServiceProvider.GetRequiredService<CreateMailerUseCase>();
            CreateMailerArgs args = new(email, password, ct);
            Result<Mailer> created = await createMailer.Invoke(args);
            Assert.True(created.IsFailure);
        }
    }

    [Fact]
    private async Task Create_Mailer_Invalid_Email()
    {
        await using AsyncServiceScope scope = services.Scope();
        CreateMailerUseCase createMailer = scope.ServiceProvider.GetRequiredService<CreateMailerUseCase>();
        CreateMailerArgs args = new("some invalid email", password, ct);
        Result<Mailer> created = await createMailer.Invoke(args);
        Assert.True(created.IsFailure);
    }

    [Fact]
    private async Task Create_Mailer_Invalid_Password()
    {
        await using AsyncServiceScope scope = services.Scope();
        CreateMailerUseCase createMailer = scope.ServiceProvider.GetRequiredService<CreateMailerUseCase>();
        CreateMailerArgs args = new(email, "    ", ct);
        Result<Mailer> created = await createMailer.Invoke(args);
        Assert.True(created.IsFailure);
    }
}