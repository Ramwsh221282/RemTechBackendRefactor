using Drom.Parsing.Vehicles.Parsing.Utilities;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.Models;

public sealed class DromCatalogueCarsCollection(IPage page)
{
    private readonly string _carsListSelector = string.Intern("div[data-bulletin-list='true']");
    private readonly string _carItemSelector = string.Intern("div[data-ftid='bulls-list_bull']");
    private readonly string _carItemImageSelector = string.Intern("div[data-ftid='bull_image']");
    private readonly string _carItemImageSlidesSelector = string.Intern("img");
    private readonly string _carItemTitleSelector = string.Intern("a[data-ftid='bull_title']");
    private const StringSplitOptions SplitOptions =
        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;

    public async Task<IEnumerable<DromCatalogueCar>> Iterate()
    {
        LinkedList<DromCatalogueCar> materialized = await Materialize();
        return materialized;
    }

    private async Task<LinkedList<DromCatalogueCar>> Materialize()
    {
        LinkedList<DromCatalogueCar> materialized = [];
        IElementHandle[] cars = await GetCars();
        foreach (IElementHandle car in cars)
        {
            string[] carImages = await GetCarImages(car);
            string sourceUrl = await GetCarSourceUrl(car);
            string id = GetCarIdFromSourceUrl(sourceUrl);
            materialized.AddFirst(new DromCatalogueCar(id, sourceUrl, carImages));
        }

        return materialized;
    }

    private async Task<IElementHandle[]> GetCars()
    {
        try
        {
            IElementHandle carList = await new ValidSingleElementSource(
                new PageElementSource(page)
            ).Read(_carsListSelector);
            IElementHandle[] cars = await new ParentManyElementsSource(carList).Read(
                _carItemSelector
            );
            return cars;
        }
        catch
        {
            throw new DromCatalogueNoItemsException();
        }
    }

    private async Task<string> GetCarSourceUrl(IElementHandle car)
    {
        IElementHandle titleElement = await new ValidSingleElementSource(
            new ParentElementSource(car)
        ).Read(_carItemTitleSelector);
        string hrefAttribute = await new AttributeFromWebElement(titleElement, "href").Read();
        return hrefAttribute;
    }

    private static string GetCarIdFromSourceUrl(string sourceUrl)
    {
        string id = $"DROM{sourceUrl.Split('/')[^1].Split('.')[0]}";
        return id;
    }

    private async Task<string[]> GetCarImages(IElementHandle car)
    {
        IElementHandle carImage = await new ParentElementSource(car).Read(_carItemImageSelector);
        IElementHandle[] carImageSlides = await new ParentManyElementsSource(carImage).Read(
            _carItemImageSlidesSelector
        );
        List<string> carImages = [];
        foreach (IElementHandle carImageSlide in carImageSlides)
        {
            string attribute = await new AttributeFromWebElement(carImageSlide, "srcset").Read();
            if (string.IsNullOrWhiteSpace(attribute))
                continue;

            string photo = attribute.Split(',', SplitOptions)[^1].Split(' ', SplitOptions)[0];
            carImages.Add(photo);
        }
        return carImages.ToArray();
    }
}
