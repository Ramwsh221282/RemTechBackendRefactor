using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.Utilities;

public sealed class DromImagesHoveringAction(IPage page)
{
    private const string CarsListSelector = "div[data-bulletin-list='true']";
    private const string CarItemSelector = "div[data-ftid='bulls-list_bull']";
    private const string CarItemImageSelector = "div[data-ftid='bull_image']";
    private const string CarItemImageSlidesSelector = "img";

    public async Task<bool> Do()
    {
        try
        {
            IElementHandle[] cars = await GetCars();
            foreach (IElementHandle car in cars)
            {
                await HoverImageSlider(car);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<IElementHandle[]> GetCars()
    {
        IElementHandle carList = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(CarsListSelector);
        IElementHandle[] cars = await new ParentManyElementsSource(carList).Read(CarItemSelector);
        return cars;
    }

    private async Task HoverImageSlider(IElementHandle car)
    {
        IElementHandle carImage = await new ParentElementSource(car).Read(CarItemImageSelector);
        IElementHandle[] carImageSlides = await new ParentManyElementsSource(carImage).Read(
            CarItemImageSlidesSelector
        );
        foreach (IElementHandle carImageSlide in carImageSlides)
        {
            await carImageSlide.FocusAsync();
            await carImageSlide.HoverAsync();
        }
    }
}
