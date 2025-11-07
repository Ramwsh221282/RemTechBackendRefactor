using Mailing.Adapters.Storage;
using Mailing.Domain.Postmans;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Shared.Infrastructure.Module.DependencyInjection;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Tests.CleanWriteTests;

public sealed class CleanWriteTest(MailingTestServices services) : IClassFixture<MailingTestServices>
{
    [Fact]
    private async Task Add_Postman_Success()
    {
        await using AsyncServiceScope scope = services.Scope();
        PostmansStorageBuilder builder = scope.GetService<PostmansStorageBuilder>();
        ILogger logger = scope.GetService<ILogger>();
        PostgresDatabase db = scope.GetService<PostgresDatabase>();
        PostmansEnvelope postmans = builder.WithLogging(logger).WithSearchCriteriaCheck(db).Build();

        TestPostman postman = new(new TestPostmanMetadata(Guid.NewGuid(), "postman@mail.com", "123"));
        await postmans.Add(postman, CancellationToken.None);
    }

    [Fact]
    private async Task Create_Postman_Ensure_Created()
    {
        string email = "postman@mail.com";
        await using AsyncServiceScope scope = services.Scope();
        IPostmans postmans = scope.GetService<IPostmans>();
        TestPostman postman = new(new TestPostmanMetadata(Guid.NewGuid(), email, "123"));
        await postmans.Add(postman, CancellationToken.None);

        IPostmanCriteria criteria = new PostmanByEmailCriteria("postman@mail.com");
        ITestPostman created = await postmans.Find(criteria, CancellationToken.None);
        Assert.IsType<TestPostman>(created);
    }
}