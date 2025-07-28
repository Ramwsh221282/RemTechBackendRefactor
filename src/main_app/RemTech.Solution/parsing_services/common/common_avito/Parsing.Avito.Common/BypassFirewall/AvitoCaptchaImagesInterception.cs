using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoCaptchaImagesInterception
{
    private readonly IElementHandle? _blockedForm;
    private readonly IPage _page;
    private readonly TaskCompletionSource _tcs;
    private AvitoCaptchaImages _images;

    public AvitoCaptchaImagesInterception(IPage page, IElementHandle? blockedForm)
    {
        _tcs = new  TaskCompletionSource();
        _images = new AvitoCaptchaImages();
        _page = page;
        _blockedForm = blockedForm;
    }

    public async Task<AvitoCaptchaImages> Intercept()
    {
        if (_blockedForm == null)
            return _images;

        _page.Response += InterceptionMethod!;
        IElementHandle? button = await new PageElementSource(_page).Read(string.Intern("button"));
        if (button == null)
        {
            _page.Response -=  InterceptionMethod!;
            return _images;
        }

        await button.ClickAsync();
        Task imagesTask = _tcs.Task;
        Task timeOutTask = Task.Delay(TimeSpan.FromSeconds(10));
        Task completed = await Task.WhenAny(imagesTask, timeOutTask);
        if (completed == timeOutTask)
        {
            _tcs.SetResult();
        }
        
        _page.Response -= InterceptionMethod!;
        return _images;
    }

    private async void InterceptionMethod(object sender, ResponseCreatedEventArgs ea)
    {
        try
        {
            string requestUrl = ea.Response.Request.Url;
            if (requestUrl.Contains(string.Intern("/bg/")) ||
                requestUrl.Contains(string.Intern("/slide/")))
            {
                await ea.Response
                    .BufferAsync()
                    .AsTask()
                    .ContinueWith(async data =>
                    {
                        _images = _images.With(await data);
                    });
            }
            
            if (_images.Amount() >= 2)
                _tcs.TrySetResult();
        }
        catch
        {
            _images = new AvitoCaptchaImages();
        }
    }
}