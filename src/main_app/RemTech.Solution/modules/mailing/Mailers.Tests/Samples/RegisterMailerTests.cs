using Mailers.Core.MailersContext;
using Mailers.Core.MailersContext.ValueObjects;
using Mailers.Persistence.NpgSql;
using Mailers.Persistence.NpgSql.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Tests.Samples;

public sealed class RegisterMailerTests(MailersTestsServices services) : IClassFixture<MailersTestsServices>
{
    [Fact]
    private async Task Register_Mailer_Success()
    {
        await using var scope = services.Scope();
        var factory = scope.ServiceProvider.GetRequiredService<NpgSqlConnectionFactory>();
        var target = new NpgMailerRegisteredEventTarget(new NpgSqlMailer(factory), CancellationToken.None);
        var metadata = MailerMetadata.Create("test@mail.com", "dsda-dsadas-dsadas");
        var mailer = Mailer.Create(metadata);
        Assert.True(mailer.IsSuccess);
        target.Listen(mailer.Value);
        mailer.Value.Register();
        var result = await target.Read();
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Register_Mailer_Email_Duplicate()
    {
        string email = "test@mail.com";
        await using var scope = services.Scope();
        var factory = scope.ServiceProvider.GetRequiredService<NpgSqlConnectionFactory>();
        var target = new NpgMailerRegisteredEventTarget(new NpgSqlMailer(factory), CancellationToken.None);
        var metadata = MailerMetadata.Create(email, "dsda-dsadas-dsadas");
        var mailer = Mailer.Create(metadata);
        target.Listen(mailer.Value);
        mailer.Value.Register();
        await target.Read();
        
        var dupTarget = new NpgMailerRegisteredEventTarget(new NpgSqlMailer(factory), CancellationToken.None);
        var dup = Mailer.Create(metadata);
        dupTarget.Listen(dup.Value);
        dup.Value.Register();
        var result = await dupTarget.Read();
        Assert.True(result.IsFailure);
    }
    
}