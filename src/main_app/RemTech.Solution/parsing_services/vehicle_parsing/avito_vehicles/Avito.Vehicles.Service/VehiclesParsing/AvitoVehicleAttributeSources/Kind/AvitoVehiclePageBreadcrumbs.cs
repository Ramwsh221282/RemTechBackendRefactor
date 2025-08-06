using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class AvitoVehiclePageBreadcrumbs
{
    private readonly IPage _page;
    private readonly string _containerSelector = string.Intern("#bx_item-breadcrumbs");
    private readonly string _breadCrumbsSelector = string.Intern(
        "span[itemprop='itemListElement']"
    );

    public AvitoVehiclePageBreadcrumbs(IPage page) => _page = page;

    public async Task<IElementHandle[]> Read()
    {
        IElementHandle breadcrumbsContainer = await new PageElementSource(_page).Read(
            _containerSelector
        );
        return await new ParentManyElementsSource(breadcrumbsContainer).Read(_breadCrumbsSelector);
    }
}
