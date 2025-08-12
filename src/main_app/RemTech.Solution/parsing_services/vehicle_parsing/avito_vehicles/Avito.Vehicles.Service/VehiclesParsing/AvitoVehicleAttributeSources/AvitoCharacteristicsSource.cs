using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources;

public sealed class AvitoCharacteristicsSource(IPage page) : IAvitoCharacteristicsSource
{
    private const string Container = "#bx_item-params";
    private const string List = ".HRzg1";
    private const string ListItem = "li";

    public async Task<IElementHandle[]> Read()
    {
        IElementHandle ctxContainer = await new PageElementSource(page).Read(Container);
        IElementHandle ctxList = await new ParentElementSource(ctxContainer).Read(List);
        return await new ParentManyElementsSource(ctxList).Read(ListItem);
    }
}
