using Mailers.Persistence.NpgSql;

namespace Mailers.Tests;

public sealed class RemoveMailerTests(MailersTestsServices services) : IClassFixture<MailersTestsServices>
{
    [Fact]
    private async Task Remove_Mailer_Success()
    {
        await using (var scope1 = services.Scope())
        {
            var ct = CancellationToken.None;
            var createMailer = scope1.ServiceProvider.GetRequiredService<IFunction<CreateMailerFunctionArgument, Result<Mailer>>>();
            var insertMailer = scope1.ServiceProvider.GetRequiredService<IAsyncFunction<InsertMailerFunctionArgument, Result<Unit>>>();
            var session1 = scope1.ServiceProvider.GetRequiredService<NpgSqlSession>();
            var metadata = new CreateMailerMetadataArguments("mailer@gmail.com", "dsadsadsa-dsadsad");
            var statistics = new CreateMailerStatisticsFunctionArgument();
            var mailer = createMailer.Invoke(new CreateMailerFunctionArgument(metadata, statistics));
            await insertMailer.Invoke(new InsertMailerFunctionArgument(session1, mailer), ct);
            
            await using (var scope2 = services.Scope())
            {
                await using var session2 = scope2.ServiceProvider.GetRequiredService<NpgSqlSession>();
                var deleteMailer = scope2.ServiceProvider.GetRequiredService<IAsyncFunction<DeleteMailerFunctionArgument, Result<Unit>>>();
                var deletion = await deleteMailer.Invoke(new DeleteMailerFunctionArgument(mailer, session2), ct);
                Assert.True(deletion.IsSuccess);
            }
            
            await using (var scope3 = services.Scope())
            {
                await using var session3 = scope3.ServiceProvider.GetRequiredService<NpgSqlSession>();
                var query = new QueryMailerArguments(Id: mailer.Value.Metadata.Id);
                var deleted = await query.Get(session3, ct);
                Assert.False(deleted.HasValue);
            }
        }
    }
}

public abstract class UserBase
{
    protected readonly string _userName;
    protected readonly int _userAge;

    public UserBase(UserBase other)
    {
        _userName = other._userName;
        _userAge = other._userAge;
    }

    public UserBase(string userName, int userAge)
    {
        _userName = userName;
        _userAge = userAge;
    }
}

public abstract class BehavioralUser : UserBase
{
    protected BehavioralUser(UserBase other) : base(other)
    {
    }

    protected BehavioralUser(string userName, int userAge) : base(userName, userAge)
    {
    }

    public virtual void Activate()
    {
        Console.WriteLine("User activated");
    }
}