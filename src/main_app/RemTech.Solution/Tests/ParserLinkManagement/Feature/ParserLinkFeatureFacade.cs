using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Tests.ParserLinkManagement.Feature;

public sealed class ParserLinkFeatureFacade(IServiceProvider sp)
{
    public async Task<Result<ParserLinkResponse>> AttachParser(
        Guid parserId, 
        string name, 
        string url)
    {
        return await new AttachParserLinkFeature(sp).Invoke(parserId, name, url);
    }

    public async Task<Result<ParserLinkResponse>> ChangeLinkUrl(Guid id, string newUrl)
    {
        return await new ChangeParserLinkUrlFeature(sp).Invoke(id, newUrl);
    }

    public async Task<Result<ParserLinkResponse>> IgnoreParserLink(Guid id)
    {
        return  await new IgnoreParserLinkFeature(sp).Invoke(id);
    }

    public async Task<Result<ParserLinkResponse>> RenameParserLink(Guid id, string newName)
    {
        return await new RenameParserLinkFeature(sp).Invoke(id, newName);
    }

    public async Task<Result<ParserLinkResponse>> UnignoreLink(Guid id)
    {
        return await new UnignoreParserLinkFeature(sp).Invoke(id);
    }
}