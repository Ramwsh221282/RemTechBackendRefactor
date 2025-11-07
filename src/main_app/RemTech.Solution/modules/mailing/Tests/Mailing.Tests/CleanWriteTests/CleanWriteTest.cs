using Mailing.Tests.CleanWriteTests.Models;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Async;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Mailing.Tests.CleanWriteTests;

public sealed class CleanWriteTest(MailingTestServices services) : IClassFixture<MailingTestServices>
{
    [Fact]
    private async Task Test()
    {
        await using AsyncServiceScope scope = services.Scope();
        IPostmans postmans = scope.GetService<IPostmans>();
        TestPostman postman = new(new TestPostmanMetadata(Guid.NewGuid(), "postman@mail.com", "123"));
        Future save = postmans.Save(postman, CancellationToken.None);
        await save.Complete();
    }
}