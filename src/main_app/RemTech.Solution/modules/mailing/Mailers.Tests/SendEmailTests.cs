using Mailers.Persistence.NpgSql;

namespace Mailers.Tests;

public sealed class SendEmailTests(MailersTestsServices services) : IClassFixture<MailersTestsServices>
{
    [Fact]
    private async Task Ping_Mailer_Success()
    {
        await using var scope = services.Scope();
        await using var session = scope.ServiceProvider.GetRequiredService<NpgSqlSession>();
        var createMailer = scope.ServiceProvider.GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("mail@email.com", "sdaddsa-dsadsads-dasdas");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        var statisticsBeforePing = mailer.Value.Statistics;
        var pingMailer = scope.ServiceProvider.GetRequiredService<IFunction<PingMailerSenderFunctionArgument, Result<MailerSending>>>();
        var pinging = pingMailer.Invoke(new PingMailerSenderFunctionArgument(mailer, "myEmail@mail.com"));
        Assert.True(pinging.IsSuccess);
        var statisticsAfterPing = pinging.Value.Mailer.Statistics;
        Assert.NotEqual(statisticsBeforePing, statisticsAfterPing);
        Assert.NotEqual(statisticsBeforePing.SendCurrent, statisticsAfterPing.SendCurrent);
    }

    [Theory]
    [InlineData("not-email_address")]
    [InlineData("")]
    private async Task Ping_Mailer_Invalid_Email_Failure(string email)
    {
        await using var scope = services.Scope();
        await using var session = scope.ServiceProvider.GetRequiredService<NpgSqlSession>();
        var createMailer = scope.ServiceProvider.GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
        var metaArgs = new CreateMailerMetadataArguments("mail@email.com", "sdaddsa-dsadsads-dasdas");
        var statArgs = new CreateMailerStatisticsFunctionArgument();
        var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
        var pingMailer = scope.ServiceProvider.GetRequiredService<IFunction<PingMailerSenderFunctionArgument, Result<MailerSending>>>();
        var pinging = pingMailer.Invoke(new PingMailerSenderFunctionArgument(mailer, email));
        Assert.True(pinging.IsFailure);
    }

    [Fact]
    private async Task Send_Mailer_Email_Ensure_Current_Send_Increased()
    {
        var ct = CancellationToken.None;
        await using (var scope1 = services.Scope())
        {
            await using var session1 = scope1.ServiceProvider.GetRequiredService<NpgSqlSession>();
            var createMailer = scope1.ServiceProvider.GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
            var insertMailer = scope1.ServiceProvider.GetRequiredService<IAsyncFunction<InsertMailerFunctionArgument, Result<Unit>>>();
            var metaArgs = new CreateMailerMetadataArguments("mail@email.com", "sdaddsa-dsadsads-dasdas");
            var statArgs = new CreateMailerStatisticsFunctionArgument();
            var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metaArgs, statArgs));
            var mailerInsertion = await insertMailer.Invoke(new InsertMailerFunctionArgument(session1, mailer), ct);
            Assert.True(mailerInsertion.IsSuccess);
            var statistics = mailer.Value.Statistics;
            
            await using (var scope2 = services.Scope())
            {
                var pingMailer = scope2.ServiceProvider.GetRequiredService<IFunction<SendEmailFunctionArgument, Result<MailerSending>>>();
                var saveInserting = scope2.ServiceProvider.GetRequiredService<IAsyncFunction<InsertMailerSendingFunctionArguments, Result<MailerSending>>>();
                await using var session2 = scope1.ServiceProvider.GetRequiredService<NpgSqlSession>();
                var pinging = pingMailer.Invoke(new SendEmailFunctionArgument(mailer, "toMe@email.com", "test Subject", "test Body"));
                Assert.True(pinging.IsSuccess);
                var pingingInsert = await saveInserting.Invoke(new InsertMailerSendingFunctionArguments(pinging, session2), ct);
                Assert.True(pingingInsert.IsSuccess);
            }
            
            await using (var scope3 = services.Scope())
            {
                await using var session3 = scope3.ServiceProvider.GetRequiredService<NpgSqlSession>();
                var mailerAfterSend = await new QueryMailerArguments(Id: mailer.Value.Metadata.Id).Get(session3, ct);
                Assert.True(mailerAfterSend.HasValue);
                Assert.NotEqual(statistics, mailerAfterSend.Value.Statistics);
                Assert.NotEqual(statistics.SendCurrent, mailerAfterSend.Value.Statistics.SendCurrent);
            }
        }
    }
}