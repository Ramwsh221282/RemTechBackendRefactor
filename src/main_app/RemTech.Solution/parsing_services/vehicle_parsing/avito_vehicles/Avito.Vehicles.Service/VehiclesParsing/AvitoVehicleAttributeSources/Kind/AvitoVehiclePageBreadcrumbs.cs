using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class AvitoVehiclePageBreadcrumbs
{
    private readonly IPage _page;
    private const string ContainerSelector = "#bx_item-breadcrumbs";
    private const string BreadCrumbsSelector = "span[itemprop='itemListElement']";

    public AvitoVehiclePageBreadcrumbs(IPage page) => _page = page;

    public async Task<IElementHandle[]> Read()
    {
        IElementHandle breadcrumbsContainer = await new PageElementSource(_page).Read(
            ContainerSelector
        );
        return await new ParentManyElementsSource(breadcrumbsContainer).Read(BreadCrumbsSelector);
    }
}
