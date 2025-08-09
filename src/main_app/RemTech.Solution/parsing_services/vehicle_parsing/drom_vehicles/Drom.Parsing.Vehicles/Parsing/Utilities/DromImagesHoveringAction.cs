using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.Utilities;

public sealed class DromImagesHoveringAction(IPage page) : IPageAction
{
    private readonly string _carsListSelector = string.Intern("div[data-bulletin-list='true']");
    private readonly string _carItemSelector = string.Intern("div[data-ftid='bulls-list_bull']");
    private readonly string _carItemImageSelector = string.Intern("div[data-ftid='bull_image']");
    private readonly string _carItemImageSlidesSelector = string.Intern("img");

    public async Task Do()
    {
        IElementHandle[] cars = await GetCars();
        foreach (IElementHandle car in cars)
        {
            await HoverImageSlider(car);
        }
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

    private async Task HoverImageSlider(IElementHandle car)
    {
        try
        {
            IElementHandle carImage = await new ParentElementSource(car).Read(
                _carItemImageSelector
            );
            IElementHandle[] carImageSlides = await new ParentManyElementsSource(carImage).Read(
                _carItemImageSlidesSelector
            );
            foreach (IElementHandle carImageSlide in carImageSlides)
            {
                await carImageSlide.FocusAsync();
                await carImageSlide.HoverAsync();
            }
        }
        catch
        {
            // ignored
        }
    }
}
