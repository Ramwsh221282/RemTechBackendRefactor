using AvitoFirewallBypass;
using Microsoft.Extensions.DependencyInjection;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.CreateCataloguePageUrls;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.ExtractCatalogueItemData;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.ExtractConcreteItem;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.PrepareAvitoPage;

namespace Tests.ParsingTests;

public sealed class AvitoParsingTests(IntegrationalTestsFixture fixture) : IClassFixture<IntegrationalTestsFixture>
{
    private BrowserManagerProvider Provider { get; } = fixture.Services.GetRequiredService<BrowserManagerProvider>();
    private AvitoBypassFactory AvitoBypassFactory { get; } = fixture.Services.GetRequiredService<AvitoBypassFactory>();
    private readonly Serilog.ILogger Logger = Serilog.Log.ForContext<AvitoParsingTests>();
    const string targetUrl = "https://www.avito.ru/all/gruzoviki_i_spetstehnika/tehnika_dlya_lesozagotovki/john_deere-ASgBAgICAkRUsiyexw3W6j8?cd=1";
    
    [Fact]
    private async Task Test_Extract_Catalogue_Items()
    {
        CataloguePageUrl[] pagedUrls = Array.Empty<CataloguePageUrl>();
        
        await using BrowserManager manager = Provider.Provide();
        await using IPage page = await manager.ProvidePage();
        CreateCataloguePageUrlsCommand command = new(page, targetUrl, AvitoBypassFactory);
        pagedUrls = await command.UseLogging(Logger).Handle();
        
        List<AvitoVehicle> results = [];
        foreach (var url in pagedUrls)
        {
            await page.PerformQuickNavigation(url.Url);
            ExtractCatalogueItemDataCommand itemsCommand = new (page, url, AvitoBypassFactory);
            AvitoVehicle[] data = await itemsCommand
                .UseLogging(Logger)
                .UseResilience(manager, page, Logger).Handle();
            results.AddRange(data);
        }
        
        
        Assert.NotEmpty(results);
    }

    [Fact]
    private async Task Parse_Concrete_Items()
    {
        await using BrowserManager manager = Provider.Provide();
        await using IPage page = await manager.ProvidePage();
        await new PrepareAvitoPageCommand(page, AvitoBypassFactory).UseLogging(Logger).Handle();
        CataloguePageUrl[] pagedUrls = await new CreateCataloguePageUrlsCommand(page, targetUrl, AvitoBypassFactory).Handle();
        List<AvitoVehicle> results = [];
        foreach (var url in pagedUrls)
        {
            await page.PerformQuickNavigation(url.Url);
            await new PrepareAvitoPageCommand(page, AvitoBypassFactory)
                .UseLogging(Logger)
                .UseResilience(manager, page)
                .Handle();
            AvitoVehicle[] data = await new ExtractCatalogueItemDataCommand(page, url, AvitoBypassFactory)
                .UseLogging(Logger)
                .UseResilience(manager, page, Logger)
                .Handle();
            
            for (int j = 0; j < data.Length; j++)
            {
                AvitoVehicle vehicle = data[j];
                vehicle = await new ExtractConcreteItemCommand(page, vehicle, AvitoBypassFactory)
                    .UseLogging(Logger)
                    .UseResilience(manager, page)
                    .Handle();
                data[j] = vehicle;
            }
            
            results.AddRange(data);
        }

        AvitoVehicle[] processed = results.Where(v => v.ConcretePageRepresentation.IsEmpty() == false).ToArray();
        Assert.NotEmpty(processed);
    }
}