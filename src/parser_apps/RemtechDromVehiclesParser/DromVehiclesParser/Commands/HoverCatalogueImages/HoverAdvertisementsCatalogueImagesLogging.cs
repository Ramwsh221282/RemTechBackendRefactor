using DromVehiclesParser.Parsing.CatalogueParsing.Models;

namespace DromVehiclesParser.Commands.HoverCatalogueImages;

public sealed class HoverAdvertisementsCatalogueImagesLogging(Serilog.ILogger logger, IHoverAdvertisementsCatalogueImagesCommand inner)
    : IHoverAdvertisementsCatalogueImagesCommand
{
    private Serilog.ILogger Logger { get; } = logger.ForContext<IHoverAdvertisementsCatalogueImagesCommand>();
    
    public async Task Hover(DromCataloguePage dromCataloguePage)
    {
        Logger.Information("Hovering catalogue images for page {Url}", dromCataloguePage.Url);
        try
        {
            await inner.Hover(dromCataloguePage);
            Logger.Information("Catalogue images for page {Url} hovered", dromCataloguePage.Url);
        }
        catch(Exception ex)
        {
            Logger.Error(ex, "Failed to hover catalogue images for page {Url}", dromCataloguePage.Url);
            throw;
        }
    }
}