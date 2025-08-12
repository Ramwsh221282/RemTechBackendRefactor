using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;
using PuppeteerSharp.Input;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoCaptchaSliderMovement(IPage page, int centerPoint) : IPageAction
{
    private const string Slider = ".geetest_btn";

    public async Task Do()
    {
        IElementHandle? slider = await new PageElementSource(page).Read(Slider);
        if (slider == null)
            return;

        await Task.Delay(TimeSpan.FromSeconds(5));
        MoveOptions moveOptions = new() { Steps = 25 };
        BoundingBox bbox = await slider.BoundingBoxAsync();
        decimal xPosition = bbox.X + (bbox.Width / 2);
        decimal yPosition = bbox.Y + (bbox.Height / 2);
        await page.Mouse.MoveAsync(xPosition, yPosition, moveOptions);
        await page.Mouse.DownAsync();
        decimal xPositionN = bbox.X + centerPoint;
        decimal yPositionN = bbox.Y;
        await page.Mouse.MoveAsync(xPositionN, yPositionN, moveOptions);
        await Task.Delay(500);
        await page.Mouse.UpAsync();
        await Task.Delay(TimeSpan.FromSeconds(10));
    }
}
