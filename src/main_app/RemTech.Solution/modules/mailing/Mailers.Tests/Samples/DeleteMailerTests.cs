using Mailers.Core.MailersContext;
using Mailers.Core.MailersContext.ValueObjects;
using Mailers.Persistence.NpgSql;
using Mailers.Persistence.NpgSql.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Tests.Samples;

public sealed class DeleteMailerTests(MailersTestsServices services) : IClassFixture<MailersTestsServices>
{
    [Fact]
    private async Task Delete_Mailer_Success()
    {
        await using var scope = services.Scope();
        var factory = scope.ServiceProvider.GetRequiredService<NpgSqlConnectionFactory>();
        await using NpgSqlMailer pgMailer = new NpgSqlMailer(factory);
        var target = new NpgMailerRegisteredEventTarget(new NpgSqlMailer(factory), CancellationToken.None);
        var metadata = MailerMetadata.Create("test@mail.com", "dsda-dsadas-dsadas");
        var mailer = Mailer.Create(metadata);
        target.Listen(mailer.Value);
        mailer.Value.Register();
        await target.Read();
        
        var deletionTarget = new NpgMailerDeletionEventTarget(new NpgSqlMailer(factory), CancellationToken.None);
        deletionTarget.Listen(mailer.Value);
        mailer.Value.Register();
        var result = await deletionTarget.Read();
        Assert.True(result.IsSuccess);
    }
}