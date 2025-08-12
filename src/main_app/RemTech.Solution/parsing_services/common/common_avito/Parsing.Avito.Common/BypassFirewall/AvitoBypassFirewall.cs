using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoBypassFirewall(IPage page) : IAvitoBypassFirewall
{
    private const string Form = ".form-action";

    public async Task<bool> Read()
    {
        IElementHandle? form = await new PageElementSource(page).Read(Form);
        AvitoCaptchaImagesInterception interception = new(page, form);
        AvitoCaptchaImages images = await interception.Intercept();
        AvitoCaptchaSliderMovementPosition position = new(images);
        await new AvitoCaptchaSliderMovement(page, position.CenterPoint()).Do();
        return false;
    }
}
