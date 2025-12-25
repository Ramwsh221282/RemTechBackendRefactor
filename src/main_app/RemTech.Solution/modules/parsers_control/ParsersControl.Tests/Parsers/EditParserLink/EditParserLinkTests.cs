using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.Features.EditLinkUrlInfo;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Tests.Parsers.EditParserLink;

public sealed class EditParserLinkTests(IntegrationalTestsFixture fixture) : IClassFixture<IntegrationalTestsFixture>
{
    private IServiceProvider Services { get; } = fixture.Services;

    [Fact]
    private async Task Edit_Parser_Link_Success()
    {
        string type = "Some type";
        string domain = "Some domain";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
        Assert.True(result.IsSuccess);
        Result<SubscribedParserLink> linkResult = await Services.AddLink(id, "Some url", "Some name");
        Assert.True(linkResult.IsSuccess);
        Result<SubscribedParserLink> editResult = await EditLink(id, linkResult.Value.Id.Value, "New name", "New url");
        Assert.True(editResult.IsSuccess);
        
        Result<ISubscribedParser> parser = await Services.GetParser(id);
        Assert.True(parser.IsSuccess);
        Result<SubscribedParserLink> link = parser.Value.FindLink(linkResult.Value.Id);
        Assert.True(link.IsSuccess);
        Assert.Equal("New name", link.Value.UrlInfo.Name);
        Assert.Equal("New url", link.Value.UrlInfo.Url);
    }
    
    private async Task<Result<SubscribedParserLink>> EditLink(Guid parserId, Guid linkId, string newName, string newUrl)
    {
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        ICommandHandler<EditLinkUrlInfoCommand, SubscribedParserLink> handler =
            scope.ServiceProvider.GetRequiredService<ICommandHandler<EditLinkUrlInfoCommand, SubscribedParserLink>>();
        EditLinkUrlInfoCommand command = new(parserId, linkId, newName, newUrl);
        return await handler.Execute(command);
    }
}