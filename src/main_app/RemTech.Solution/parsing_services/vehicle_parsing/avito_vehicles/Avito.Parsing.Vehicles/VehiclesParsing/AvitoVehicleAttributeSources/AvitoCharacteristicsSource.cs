using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources;

public sealed class AvitoCharacteristicsSource(IPage page)
{
    public async Task<IElementHandle[]> Read()
    {
        IElementHandle? ctxContainer = await new PageElementSource(page)
            .Read(string.Intern("#bx_item-params"));
        if (ctxContainer == null)
            return [];
        IElementHandle? ctxList = await new ParentElementSource(ctxContainer)
            .Read(string.Intern(".HRzg1"));
        if (ctxList == null)
            return [];
        return await new ParentManyElementsSource(ctxList).Read("li");
    }
}