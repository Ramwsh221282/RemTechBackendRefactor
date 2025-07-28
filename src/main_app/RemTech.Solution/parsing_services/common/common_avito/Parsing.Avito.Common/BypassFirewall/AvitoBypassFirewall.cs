using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoBypassFirewall : IAvitoBypassFirewall
{
    private readonly IPage _page;

    public AvitoBypassFirewall(IPage page) =>
        _page = page;

    public async  Task<bool> Read()
    {
        string firewallForm = string.Intern(".form-action");
        IElementHandle? form = await new PageElementSource(_page).Read(firewallForm);
        AvitoCaptchaImagesInterception interception = new(_page, form);
        AvitoCaptchaImages images = await interception.Intercept();
        AvitoCaptchaSliderMovementPosition position = new(images);
        await new AvitoCaptchaSliderMovement(_page, position.CenterPoint()).Do();
        return false;
    }
}