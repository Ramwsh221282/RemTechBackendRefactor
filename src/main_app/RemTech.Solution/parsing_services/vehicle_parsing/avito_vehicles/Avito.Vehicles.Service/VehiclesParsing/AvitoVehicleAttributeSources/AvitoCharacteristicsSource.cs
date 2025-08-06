using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources;

public interface IAvitoCharacteristicsSource
{
    Task<IElementHandle[]> Read();
}

public sealed class DefaultOnErrorAvitoCharacteristics : IAvitoCharacteristicsSource
{
    private readonly IAvitoCharacteristicsSource _origin;

    public DefaultOnErrorAvitoCharacteristics(IAvitoCharacteristicsSource origin)
    {
        _origin = origin;
    }

    public async Task<IElementHandle[]> Read()
    {
        try
        {
            return await _origin.Read();
        }
        catch
        {
            return [];
        }
    }
}

public sealed class AvitoCharacteristicsSource(IPage page) : IAvitoCharacteristicsSource
{
    public async Task<IElementHandle[]> Read()
    {
        IElementHandle ctxContainer = await new PageElementSource(page).Read(
            string.Intern("#bx_item-params")
        );
        IElementHandle ctxList = await new ParentElementSource(ctxContainer).Read(
            string.Intern(".HRzg1")
        );
        return await new ParentManyElementsSource(ctxList).Read("li");
    }
}
