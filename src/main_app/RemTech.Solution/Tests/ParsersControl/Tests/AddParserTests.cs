using ParsersControl.Presenters.ParserRegistrationManagement.AddParser;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.ParsersControl.Features;

namespace Tests.ParsersControl.Tests;

public sealed class AddParserTests(ParsersControlFixture fixture) : IClassFixture<ParsersControlFixture>
{
    private readonly RegisteredParsersFacade _facade = new(fixture.Services);

    [Fact]
    private async Task Register_Parser_Success()
    {
        const string domain = "my-domain";
        const string type = "transport";
        Result<AddParserResponse> result = await _facade.AddParser(domain, type);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Register_Parser_Domain_Invalid()
    {
        const string domain = "   ";
        const string type = "transport";
        Result<AddParserResponse> result = await _facade.AddParser(domain, type);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Register_Parser_Type_Invalid()
    {
        const string domain = "my-domain";
        const string type = "   ";
        Result<AddParserResponse> result = await _facade.AddParser(domain, type);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Register_Parser_Duplicate_Domain_Type()
    {
        const string domain = "my-domain";
        const string type = "transport";
        await _facade.AddParser(domain, type);
        Result<AddParserResponse> result = await _facade.AddParser(domain, type);
        Assert.True(result.IsFailure);
    }
}