using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.Parsers.Contracts;
using ParsersControl.Core.Parsers.Features.SubscribeParser;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Tests.Parsers.SubscribeParser;

public sealed class SubscribeParserTests(IntegrationalTestsFixture fixture) : IClassFixture<IntegrationalTestsFixture>
{
    private IServiceProvider Services { get; } = fixture.Services;

    [Fact]
    private async Task Subscribe_Parser_Ensure_Subscribed()
    {
        string domain = "Some Domain";
        string type = "Some Type";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await InvokeSubscription(domain, type, id);
        Assert.True(result.IsSuccess);
        bool saved = await EnsureSaved(id);
        Assert.True(saved);
    }
    
    [Fact]
    private async Task Subscribe_Parser_Duplication_By_Domain_And_Type()
    {
        string domain = "Some Domain";
        string type = "Some Type";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await InvokeSubscription(domain, type, id);
        Assert.True(result.IsSuccess);
        Result<SubscribedParser> result2 = await InvokeSubscription(domain, type, id);
        Assert.False(result2.IsSuccess);
    }
    
    [Fact]
    private async Task Susbscribe_Parser_Invalid_Domain()
    {
        string domain = "";
        string type = "Some Type";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await InvokeSubscription(domain, type, id);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    private async Task Subscribe_Parser_Invalid_Type()
    {
        string domain = "Some Domain";
        string type = "";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await InvokeSubscription(domain, type, id);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    private async Task Subscribe_Parser_Invalid_Id()
    {
        string domain = "Some Domain";
        string type = "Some Type";
        Guid id = Guid.Empty;
        Result<SubscribedParser> result = await InvokeSubscription(domain, type, id);
        Assert.False(result.IsSuccess);
    }

    private async Task<bool> EnsureSaved(Guid id)
    {
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        ISubscribedParsersRepository repository = scope.ServiceProvider.GetRequiredService<ISubscribedParsersRepository>();
        Result<ISubscribedParser> parser = await repository.Get(new SubscribedParserQuery(Id: id));
        return parser.IsSuccess;
    }
    
    private async Task<Result<SubscribedParser>> InvokeSubscription(string domain, string type, Guid id)
    {
        SubscribeParserCommand command = new(id, domain, type);
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        ICommandHandler<SubscribeParserCommand, SubscribedParser> handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<SubscribeParserCommand, SubscribedParser>>();
        return await handler.Execute(command);
    }
}