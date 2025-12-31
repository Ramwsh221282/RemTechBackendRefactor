using AvitoFirewallBypass;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing;
using RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing.Extensions;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.CreateCataloguePageUrls;

public sealed class CreateCataloguePageUrlsCommand(Func<Task<IPage>> pageSource, string url, AvitoBypassFactory bypassFactory) : ICreateCataloguePageUrlsCommand
{
    public async Task<CataloguePageUrl[]> Handle()
    {
        IPage page = await pageSource();
        IAvitoBypassFirewall bypass = bypassFactory.Create(page);
        string[] urls = await new AvitoPagedUrlsExtractor(page, bypass).ExtractUrls(url);
        return urls.Select(CataloguePageUrl.New).ToArray();
    }
}