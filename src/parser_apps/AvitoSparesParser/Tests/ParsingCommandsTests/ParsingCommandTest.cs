using AvitoFirewallBypass;
using AvitoSparesParser.AvitoSpareContext;
using AvitoSparesParser.CatalogueParsing;
using AvitoSparesParser.Commands.ExtractCataloguePageItems;
using AvitoSparesParser.Commands.ExtractConcretePageItem;
using AvitoSparesParser.Commands.ExtractPagedUrls;
using Microsoft.Extensions.DependencyInjection;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace Tests.ParsingCommandsTests;

public sealed class ParsingCommandTest(IntegrationalTestsFixture fixture)
    : IClassFixture<IntegrationalTestsFixture>
{
    private BrowserManagerProvider BrowserProvider { get; } =
        fixture.Services.GetRequiredService<BrowserManagerProvider>();
    private AvitoBypassFactory Bypasses { get; } =
        fixture.Services.GetRequiredService<AvitoBypassFactory>();
    private Serilog.ILogger Logger { get; } =
        fixture.Services.GetRequiredService<Serilog.ILogger>();

    private const string TargetUrl =
        "https://www.avito.ru/all/zapchasti_i_aksessuary/zapchasti/dlya_gruzovikov_i_spetstehniki/texnika_dlia_lesozagotovki-ASgBAgICA0QKJKwJjGT46w7G1oED?cd=1&f=ASgBAgICBEQKJKwJjGSexw346j_46w7G1oED";

    [Fact]
    private async Task Extract_Catalogue_Paged_Urls()
    {
        AvitoCataloguePage[] pages = Array.Empty<AvitoCataloguePage>();
        
        await using (BrowserManager manager = BrowserProvider.Provide())
        {
            await using (IPage page = await manager.ProvidePage())
            {
                pages = await new ExtractPagedUrlsCommand(page, Bypasses)
                    .UseLogging(Logger)
                    .Extract(TargetUrl);
            }
        }
        
        Assert.NotEmpty(pages);
    }

    [Fact]
    private async Task Extract_Catalogue_Page_Items()
    {
        List<AvitoSpare> items = [];
        await using (BrowserManager manager = BrowserProvider.Provide())
        {
            await using (IPage page = await manager.ProvidePage())
            {
                AvitoCataloguePage[] pages = await new ExtractPagedUrlsCommand(page, Bypasses)
                    .UseLogging(Logger)
                    .UseResilience(Logger, manager, page)
                    .Extract(TargetUrl);
                
                foreach (AvitoCataloguePage cataloguePage in pages)
                {
                    AvitoSpare[] result = await new ExtractCataloguePagesItemCommand(page, Bypasses)
                        .UseLogging(Logger)
                        .UseResilience(Logger, manager, page)
                        .Extract(cataloguePage);
                    
                    items.AddRange(result);
                }
            }
        }
        
        Assert.NotEmpty(items);
    }

    [Fact]
    private async Task Extract_Concrete_Item()
    {
        List<AvitoSpare> results = [];
        
        await using (BrowserManager manager = BrowserProvider.Provide())
        {
            await using (IPage page = await manager.ProvidePage())
            {
                AvitoCataloguePage[] pages = await new ExtractPagedUrlsCommand(page, Bypasses)
                    .UseLogging(Logger)
                    .UseResilience(Logger, manager, page)
                    .Extract(TargetUrl);
                
                List<AvitoSpare> items = [];
                
                foreach (AvitoCataloguePage cataloguePage in pages)
                {
                    AvitoSpare[] result = await new ExtractCataloguePagesItemCommand(page, Bypasses)
                        .UseLogging(Logger)
                        .UseResilience(Logger, manager, page)
                        .Extract(cataloguePage);
                    
                    items.AddRange(result);
                }

                foreach (AvitoSpare spare in items)
                {
                    AvitoSpare result = await new ExtractConcretePageItemCommand(page, Bypasses)
                        .UseLogging(Logger)
                        .UseResilience(Logger, manager, page)
                        .Extract(spare);
                    
                    results.Add(result);
                }
            }
        }
        
        Assert.NotEmpty(results);
    }
}
