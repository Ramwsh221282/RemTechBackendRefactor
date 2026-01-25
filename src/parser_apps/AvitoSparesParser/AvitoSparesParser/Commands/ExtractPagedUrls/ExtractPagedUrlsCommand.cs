using AvitoSparesParser.CatalogueParsing;
using AvitoSparesParser.CatalogueParsing.Extensions;
using PuppeteerSharp;

namespace AvitoSparesParser.Commands.ExtractPagedUrls;

public sealed class ExtractPagedUrlsCommand(
    Func<Task<IPage>> pageSource, 
    AvitoBypassFactory bypassFactory
    ) : IExtractPagedUrlsCommand
{
    public async Task<AvitoCataloguePage[]> Extract(string initialUrl)
    {
        IPage page = await pageSource();
        IAvitoBypassFirewall bypass = bypassFactory.Create(page);
        string[] pagedUrls = await new AvitoPagedUrlsExtractor(page, bypass).ExtractUrls(initialUrl);
        return pagedUrls.Select(AvitoCataloguePage.New).ToArray();
    }
}