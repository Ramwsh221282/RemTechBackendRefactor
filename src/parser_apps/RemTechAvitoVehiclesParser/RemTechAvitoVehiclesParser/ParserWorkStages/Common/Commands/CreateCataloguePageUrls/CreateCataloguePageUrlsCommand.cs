using AvitoFirewallBypass;
using PuppeteerSharp;
using RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing;
using RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing.Extensions;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.CreateCataloguePageUrls;

public sealed class CreateCataloguePageUrlsCommand(
    IPage page,
    string url,
    AvitoBypassFactory bypassFactory
) : ICreateCataloguePageUrlsCommand
{
    public async Task<CataloguePageUrl[]> Handle()
    {
        IAvitoBypassFirewall bypass = bypassFactory.Create(page);
        string[] urls = await new AvitoPagedUrlsExtractor(page, bypass).ExtractUrls(url);
        return [.. urls.Select(CataloguePageUrl.New)];
    }
}
