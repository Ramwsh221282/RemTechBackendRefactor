using System.Runtime.InteropServices;
using DromVehiclesParser.Commands.HoverCatalogueImages;
using DromVehiclesParser.Parsing.CatalogueParsing.Models;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace DromVehiclesParser.Commands.ExtractAdvertisementsFromCatalogue;

public sealed class ExtractAdvertisementsFromCatalogueResilient(
    BrowserManager manager, 
    IPage webPage, 
    IExtractAdvertisementsFromCatalogueCommand inner, 
    int attempt = 5) : IExtractAdvertisementsFromCatalogueCommand
{
    public async Task<DromCatalogueAdvertisement[]> Extract(
        DromCataloguePage page, 
        IHoverAdvertisementsCatalogueImagesCommand hovering)
    {
        int attempts = 0;
        while (attempts < attempt)
        {
            try
            {
                return await inner.Extract(page, hovering);
            }
            catch (Exception ex) when (attempts < attempt)
            {
                webPage = await manager.RecreatePage(webPage);
                manager.ReleasePageUsedMemoryResources();
                attempts++;
            }
        }

        throw new InvalidOleVariantTypeException("Failed to extract advertisements from catalogue page");
    }
}