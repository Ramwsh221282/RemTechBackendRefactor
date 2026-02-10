using DromVehiclesParser.Commands.ExtractAdvertisementFromItsPage;
using DromVehiclesParser.Commands.ExtractAdvertisementsFromCatalogue;
using DromVehiclesParser.Commands.ExtractPagedUrls;
using DromVehiclesParser.Commands.HoverCatalogueImages;
using DromVehiclesParser.Parsing.CatalogueParsing.Models;
using DromVehiclesParser.Parsing.ConcreteItemParsing.Models;
using Microsoft.Extensions.DependencyInjection;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace Tests.ParsingCommandsTests;

public sealed class ExtractPagedUrlTests(IntegrationalTestsFixture fixture) : IClassFixture<IntegrationalTestsFixture>
{
    private IServiceProvider Services { get; } = fixture.Services;
    private Serilog.ILogger Logger { get; } = fixture.Services.GetRequiredService<Serilog.ILogger>();
    private BrowserManagerProvider BrowserProvider { get; } = fixture.Services.GetRequiredService<BrowserManagerProvider>();
     

    [Fact]
    private async Task Invoke_Extract_Paged_Url()
    {
        const string initialUrl = "https://auto.drom.ru/spec/john-deere/forestry/all/";
        await using BrowserManager manager = BrowserProvider.Provide();
        await using IPage page = await manager.ProvidePage();
        ExtractPagedUrlsCommand command = new ExtractPagedUrlsCommand(page);
        IEnumerable<DromCataloguePage> pages = await command.UseLogging(Logger).UseResilience(page, manager).Extract(initialUrl);
        Assert.NotEmpty(pages);
    }

    [Fact]
    private async Task Invoke_Extract_Catalogue_Advertisements()
    {
        const string initialUrl = "https://auto.drom.ru/spec/john-deere/forestry/all/";
        await using BrowserManager manager = BrowserProvider.Provide();
        await using IPage page = await manager.ProvidePage();
        IEnumerable<DromCataloguePage> pages = await new ExtractPagedUrlsCommand(page)
            .UseLogging(Logger)
            .UseResilience(page, manager)
            .Extract(initialUrl);
        foreach (DromCataloguePage dromCataloguePage in pages)
        {
            IHoverAdvertisementsCatalogueImagesCommand hoverCommand = new HoverAdvertisementsCatalogueImagesCommand(page)
                .UseLogging(Logger)
                .UseResilience(manager, page);
            
            IExtractAdvertisementsFromCatalogueCommand extractCommand = new ExtractAdvertisementsFromCatalogueCommand(page)
                    .UseLogging(Logger)
                    .UseResilience(manager, page);
            
            DromCatalogueAdvertisement[] results = await extractCommand.Extract(dromCataloguePage, hoverCommand);
            Assert.NotEmpty(results);
            break;
        }
        
    }

    [Fact]
    private async Task Extract_Advertisement_From_Its_Page()
    {
        const string initialUrl = "https://auto.drom.ru/spec/john-deere/forestry/all/";
        await using BrowserManager manager = BrowserProvider.Provide();
        await using IPage page = await manager.ProvidePage();
        IEnumerable<DromCataloguePage> pages = await new ExtractPagedUrlsCommand(page)
            .UseLogging(Logger)
            .Extract(initialUrl);

        foreach (DromCataloguePage dromPages in pages)
        {
            IHoverAdvertisementsCatalogueImagesCommand hoverCommand = new HoverAdvertisementsCatalogueImagesCommand(page)
                .UseLogging(Logger)
                .UseResilience(manager, page);
            
            IExtractAdvertisementsFromCatalogueCommand extractCommand = new ExtractAdvertisementsFromCatalogueCommand(page)
                .UseLogging(Logger)
                .UseResilience(manager, page);
            
            DromCatalogueAdvertisement[] results = await extractCommand.Extract(dromPages, hoverCommand);
            Assert.NotEmpty(results);
            List<DromAdvertisementFromPage> fromPageResults = [];
            
            foreach (DromCatalogueAdvertisement advertisement in results)
            {
                IExtractAdvertisementFromItsPageCommand extractFromItsPageCommand = new ExtractAdvertisementFromItsPageCommand(page)
                    .UseLogging(Logger)
                    .UseResilience(page, manager);

                try
                {
                    DromAdvertisementFromPage result = await extractFromItsPageCommand.Extract(advertisement);
                    fromPageResults.Add(result);
                }
                catch(EvaluationFailedException)
                {
                    Logger.Fatal("Evaluation exception triggered. Skipping.");
                }
                catch (Exception ex)
                {
                    Logger.Fatal(ex, "Failed to extract advertisement from page {Url}", advertisement.Url);
                }
            }
            
            Assert.NotEmpty(fromPageResults);
            break;
        }
    }
}