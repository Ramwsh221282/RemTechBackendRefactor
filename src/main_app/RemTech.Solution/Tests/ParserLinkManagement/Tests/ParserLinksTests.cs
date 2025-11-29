using ParsersControl.Presenters.ParserLinkManagement.Common;
using ParsersControl.Presenters.ParserRegistrationManagement.AddParser;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.ParserLinkManagement.Feature;
using Tests.ParsersControl.Features;

namespace Tests.ParserLinkManagement.Tests;

public sealed class ParserLinksTests(ParserLinkFixture fixture) : IClassFixture<ParserLinkFixture>
{
    private readonly ParserLinkFeatureFacade _linksFacade = new(fixture.Services);
    private readonly ParserControlFeaturesFacade _parserFacade = new(fixture.Services);

    [Fact]
    private async Task Attach_Parser_Link_Success()
    {
        const string parserDomain = "test-domain";
        const string parserType = "test-Type";
        const string testName = "test-Name";
        const string testUrl = "test-Url";
        Result<AddParserResponse> parser = await _parserFacade.AddParser(parserDomain, parserType);
        Guid id = parser.Value.Id;
        Result<ParserLinkResponse> attaching = await _linksFacade.AttachParser(id, testName, testUrl);
        Assert.True(attaching.IsSuccess);
    }
    
    [Fact]
    private async Task Attach_Parser_Link_Invalid_Name_Failure()
    {
        const string parserDomain = "test-domain";
        const string parserType = "test-Type";
        const string testName = "  ";
        const string testUrl = "test-Url";
        Result<AddParserResponse> parser = await _parserFacade.AddParser(parserDomain, parserType);
        Guid id = parser.Value.Id;
        Result<ParserLinkResponse> attaching = await _linksFacade.AttachParser(id, testName, testUrl);
        Assert.True(attaching.IsFailure);
    }
    
    [Fact]
    private async Task Attach_Parser_Link_Invalid_Url_Failure()
    {
        const string parserDomain = "test-domain";
        const string parserType = "test-Type";
        const string testName = "test-Name";
        const string testUrl = "   ";
        Result<AddParserResponse> parser = await _parserFacade.AddParser(parserDomain, parserType);
        Guid id = parser.Value.Id;
        Result<ParserLinkResponse> attaching = await _linksFacade.AttachParser(id, testName, testUrl);
        Assert.True(attaching.IsFailure);
    }

    [Fact]
    private async Task Attach_Parser_Link_Parser_Not_Found_Failure()
    {
        const string testName = "test-Name";
        const string testUrl = "test-Url";
        Result<ParserLinkResponse> attaching = await _linksFacade.AttachParser(
            Guid.NewGuid(), 
            testName, 
            testUrl);
        Assert.True(attaching.IsFailure);
    }

    [Fact]
    private async Task Rename_Parser_Link_Success()
    {
        const string parserDomain = "test-domain";
        const string parserType = "test-Type";
        const string testName = "test-Name";
        const string testUrl = "test-Url";
        const string newName = "new-Name";
        Result<AddParserResponse> parser = await _parserFacade.AddParser(parserDomain, parserType);
        Guid id = parser.Value.Id;
        Result<ParserLinkResponse> attaching = await _linksFacade.AttachParser(id, testName, testUrl);
        Assert.True(attaching.IsSuccess);
        Guid linkId = attaching.Value.Id;
        Result<ParserLinkResponse> renaming = await _linksFacade.RenameParserLink(linkId, newName);
    }

    [Fact]
    private async Task Change_Parser_Link_Url()
    {
        const string parserDomain = "test-domain";
        const string parserType = "test-Type";
        const string testName = "test-Name";
        const string testUrl = "test-Url";
        const string newUrl = "new-Url";
        Result<AddParserResponse> parser = await _parserFacade.AddParser(parserDomain, parserType);
        Guid id = parser.Value.Id;
        Result<ParserLinkResponse> attaching = await _linksFacade.AttachParser(id, testName, testUrl);
        Assert.True(attaching.IsSuccess);
        Guid linkId = attaching.Value.Id;
        Result<ParserLinkResponse> urlChanging = await _linksFacade.ChangeLinkUrl(linkId, newUrl);
        Assert.True(urlChanging.IsSuccess);
    }

    [Fact]
    private async Task Change_Parser_Link_Invalid_Url()
    {
        const string parserDomain = "test-domain";
        const string parserType = "test-Type";
        const string testName = "test-Name";
        const string testUrl = "test-Url";
        const string newUrl = "   ";
        Result<AddParserResponse> parser = await _parserFacade.AddParser(parserDomain, parserType);
        Guid id = parser.Value.Id;
        Result<ParserLinkResponse> attaching = await _linksFacade.AttachParser(id, testName, testUrl);
        Assert.True(attaching.IsSuccess);
        Guid linkId = attaching.Value.Id;
        Result<ParserLinkResponse> urlChanging = await _linksFacade.ChangeLinkUrl(linkId, newUrl);
        Assert.True(urlChanging.IsFailure);
    }

    [Fact]
    private async Task Change_Parser_Link_Url_Not_Found()
    {
        const string newUrl = "new-Url";
        Result<ParserLinkResponse> urlChanging = await _linksFacade.ChangeLinkUrl(Guid.NewGuid(), newUrl);
        Assert.True(urlChanging.IsFailure);
    }

    [Fact]
    private async Task Ignoring_Parser_Link()
    {
        const string parserDomain = "test-domain";
        const string parserType = "test-Type";
        const string testName = "test-Name";
        const string testUrl = "test-Url";
        Result<AddParserResponse> parser = await _parserFacade.AddParser(parserDomain, parserType);
        Guid id = parser.Value.Id;
        Result<ParserLinkResponse> attaching = await _linksFacade.AttachParser(id, testName, testUrl);
        Assert.True(attaching.IsSuccess);
        Guid linkId = attaching.Value.Id;
        Result<ParserLinkResponse> result = await _linksFacade.IgnoreParserLink(linkId);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Ignoring_Parser_Link_Not_Found_Failure()
    {
        Result<ParserLinkResponse> result = await _linksFacade.IgnoreParserLink(Guid.NewGuid());
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Unignoring_Parser_Link_Success()
    {
        const string parserDomain = "test-domain";
        const string parserType = "test-Type";
        const string testName = "test-Name";
        const string testUrl = "test-Url";
        Result<AddParserResponse> parser = await _parserFacade.AddParser(parserDomain, parserType);
        Guid id = parser.Value.Id;
        Result<ParserLinkResponse> attaching = await _linksFacade.AttachParser(id, testName, testUrl);
        Assert.True(attaching.IsSuccess);
        Guid linkId = attaching.Value.Id;
        Result<ParserLinkResponse> result = await _linksFacade.UnignoreLink(linkId);
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    private async Task Unignoring_Parser_Link_Not_Found_Failure()
    {
        Result<ParserLinkResponse> result = await _linksFacade.UnignoreLink(Guid.NewGuid());
        Assert.True(result.IsFailure);
    }
}