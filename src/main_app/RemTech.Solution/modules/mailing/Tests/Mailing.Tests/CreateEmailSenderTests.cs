using Mailing.CompositionRoot.Postmans;
using Mailing.Domain.Postmans;
using Mailing.Domain.Postmans.Factories;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Mailing.Tests;

public sealed class CreateEmailSenderTests : IClassFixture<MailingTestServices>
{
    private readonly MailingTestServices _services;

    public CreateEmailSenderTests(MailingTestServices services)
    {
        _services = services;
    }

    [Fact]
    private async Task Create_sender_success()
    {
        await using AsyncServiceScope scope = _services.Scope();
        CreatePostmanGateway gateway = scope.GetService<CreatePostmanGateway>();
        PostmanConstructionContext context = new(Guid.NewGuid(), "daasd-dsadsasad-dsadsaas", "my_email@mail.com");
        Status<IPostman> result = await gateway.Invoke(context, CancellationToken.None);
        Assert.True(result.IsSuccess);
    }
}