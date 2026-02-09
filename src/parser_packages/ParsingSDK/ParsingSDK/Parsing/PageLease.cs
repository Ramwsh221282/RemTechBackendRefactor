using System.Text.Json;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using PuppeteerSharp.Media;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.PageAccessibility;
using PuppeteerSharp.PageCoverage;
using ErrorEventArgs = PuppeteerSharp.ErrorEventArgs;

namespace ParsingSDK.Parsing;

public sealed class PageLease : IPage
{
    private readonly IPage _page;
    private readonly BrowserManager _browserManager;
    
    public PageLease(BrowserManager browserManager, IPage page)
    {
        _page = page;
        _browserManager = browserManager;
        SubscribeEvents();
    }
    
    public void Dispose()
    {
        UnsubscribeEvents();
        _browserManager.ReturnPageAsync(_page).Wait();
    }

    public async ValueTask DisposeAsync()
    {
        UnsubscribeEvents();
        await _browserManager.ReturnPageAsync(_page);
    }

    public Task<IElementHandle> AddScriptTagAsync(AddTagOptions options)
    {
        return _page.AddScriptTagAsync(options);
    }

    public Task<IElementHandle> AddScriptTagAsync(string url)
    {
        return _page.AddScriptTagAsync(url);
    }

    public Task<IElementHandle> AddStyleTagAsync(AddTagOptions options)
    {
        return _page.AddStyleTagAsync(options);
    }

    public Task<IElementHandle> AddStyleTagAsync(string url)
    {
        return _page.AddStyleTagAsync(url);
    }

    public Task AuthenticateAsync(Credentials credentials)
    {
        return _page.AuthenticateAsync(credentials);
    }

    public Task BringToFrontAsync()
    {
        return _page.BringToFrontAsync();
    }

    public Task ClickAsync(string selector, ClickOptions? options = null)
    {
        return _page.ClickAsync(selector, options);
    }

    public Task CloseAsync(PageCloseOptions? options = null)
    {
        return _page.CloseAsync(options);
    }

    public Task DeleteCookieAsync(params CookieParam[] cookies)
    {
        return _page.DeleteCookieAsync(cookies);
    }

    public Task EmulateAsync(DeviceDescriptor options)
    {
        return _page.EmulateAsync(options);
    }

    public Task EmulateCPUThrottlingAsync(decimal? factor = null)
    {
        return _page.EmulateCPUThrottlingAsync(factor);
    }

    public Task EmulateIdleStateAsync(EmulateIdleOverrides? idleOverrides = null)
    {
        return _page.EmulateIdleStateAsync(idleOverrides);
    }

    public Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features)
    {
        return _page.EmulateMediaFeaturesAsync(features);
    }

    public Task EmulateMediaTypeAsync(MediaType type)
    {
        return _page.EmulateMediaTypeAsync(type);
    }

    public Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions)
    {
        return _page.EmulateNetworkConditionsAsync(networkConditions);
    }

    public Task EmulateTimezoneAsync(string timezoneId)
    {
        return _page.EmulateTimezoneAsync(timezoneId);
    }

    public Task EmulateVisionDeficiencyAsync(VisionDeficiency type)
    {
        return _page.EmulateVisionDeficiencyAsync(type);
    }

    public Task<JsonElement?> EvaluateExpressionAsync(string script)
    {
        return _page.EvaluateExpressionAsync(script);
    }

    public Task<T> EvaluateExpressionAsync<T>(string script)
    {
        return _page.EvaluateExpressionAsync<T>(script);
    }

    public Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
    {
        return _page.EvaluateExpressionHandleAsync(script);
    }

    public Task<NewDocumentScriptEvaluation> EvaluateExpressionOnNewDocumentAsync(string expression)
    {
        return _page.EvaluateExpressionOnNewDocumentAsync(expression);
    }

    public Task<JsonElement?> EvaluateFunctionAsync(string script, params object[] args)
    {
        return _page.EvaluateFunctionAsync(script, args);
    }

    public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
    {
        return _page.EvaluateFunctionAsync<T>(script, args);
    }

    public Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
    {
        return _page.EvaluateFunctionHandleAsync(pageFunction, args);
    }

    public Task<NewDocumentScriptEvaluation> EvaluateFunctionOnNewDocumentAsync(string pageFunction, params object[] args)
    {
        return _page.EvaluateFunctionOnNewDocumentAsync(pageFunction, args);
    }

    public Task ExposeFunctionAsync(string name, Action puppeteerFunction)
    {
        return _page.ExposeFunctionAsync(name, puppeteerFunction);
    }

    public Task RemoveExposedFunctionAsync(string name)
    {
        return _page.RemoveExposedFunctionAsync(name);
    }

    public Task ExposeFunctionAsync<T, TResult>(string name, Func<T, TResult> puppeteerFunction)
    {
        return _page.ExposeFunctionAsync(name, puppeteerFunction);
    }

    public Task ExposeFunctionAsync<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> puppeteerFunction)
    {
        return _page.ExposeFunctionAsync(name, puppeteerFunction);
    }

    public Task ExposeFunctionAsync<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> puppeteerFunction)
    {
        return _page.ExposeFunctionAsync(name, puppeteerFunction);
    }

    public Task ExposeFunctionAsync<T1, T2, TResult>(string name, Func<T1, T2, TResult> puppeteerFunction)
    {
        return _page.ExposeFunctionAsync(name, puppeteerFunction);
    }

    public Task ExposeFunctionAsync<TResult>(string name, Func<TResult> puppeteerFunction)
    {
        return _page.ExposeFunctionAsync(name, puppeteerFunction);
    }

    public Task FocusAsync(string selector)
    {
        return _page.FocusAsync(selector);
    }

    public Task<string> GetContentAsync(GetContentOptions? options = null)
    {
        return _page.GetContentAsync(options);
    }

    public Task<CookieParam[]> GetCookiesAsync(params string[] urls)
    {
        return _page.GetCookiesAsync(urls);
    }

    public Task<string> GetTitleAsync()
    {
        return _page.GetTitleAsync();
    }

    public Task<IResponse> GoBackAsync(NavigationOptions? options = null)
    {
        return _page.GoBackAsync(options);
    }

    public Task<IResponse> GoForwardAsync(NavigationOptions? options = null)
    {
        return _page.GoForwardAsync(options);
    }

    public Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[]? waitUntil = null)
    {
        return _page.GoToAsync(url, timeout, waitUntil);
    }

    public Task<IResponse> GoToAsync(string url, NavigationOptions options)
    {
        return _page.GoToAsync(url, options);
    }

    public Task<IResponse> GoToAsync(string url, WaitUntilNavigation waitUntil)
    {
        return _page.GoToAsync(url, waitUntil);
    }

    public Task HoverAsync(string selector)
    {
        return _page.HoverAsync(selector);
    }

    public Task<Dictionary<string, decimal>> MetricsAsync()
    {
        return _page.MetricsAsync();
    }

    public Task PdfAsync(string file)
    {
        return _page.PdfAsync(file);
    }

    public Task PdfAsync(string file, PdfOptions options)
    {
        return _page.PdfAsync(file, options);
    }

    public Task<byte[]> PdfDataAsync()
    {
        return _page.PdfDataAsync();
    }

    public Task<byte[]> PdfDataAsync(PdfOptions options)
    {
        return _page.PdfDataAsync(options);   
    }

    public Task<Stream> PdfStreamAsync()
    {
        return _page.PdfStreamAsync();
    }

    public Task<Stream> PdfStreamAsync(PdfOptions options)
    {
        return _page.PdfStreamAsync(options);
    }

    public Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle)
    {
        return _page.QueryObjectsAsync(prototypeHandle);   
    }

    public Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
    {
        return _page.QuerySelectorAllAsync(selector);
    }

    public Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
    {
        return _page.QuerySelectorAllHandleAsync(selector);
    }

    public Task<IElementHandle> QuerySelectorAsync(string selector)
    {
        return _page.QuerySelectorAsync(selector);
    }

    public Task<IResponse> ReloadAsync(int? timeout = null, WaitUntilNavigation[]? waitUntil = null)
    {
        return _page.ReloadAsync(timeout, waitUntil);
    }

    public Task<IResponse> ReloadAsync(NavigationOptions options)
    {
        return _page.ReloadAsync(options);
    }

    public Task ScreenshotAsync(string file)
    {
        return _page.ScreenshotAsync(file);
    }

    public Task ScreenshotAsync(string file, ScreenshotOptions options)
    {
        return _page.ScreenshotAsync(file, options);
    }

    public Task<string> ScreenshotBase64Async()
    {
        return _page.ScreenshotBase64Async();
    }

    public Task<string> ScreenshotBase64Async(ScreenshotOptions options)
    {
        return _page.ScreenshotBase64Async(options);
    }

    public Task<byte[]> ScreenshotDataAsync()
    {
        return _page.ScreenshotDataAsync();
    }

    public Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options)
    {
        return _page.ScreenshotDataAsync(options);
    }

    public Task<Stream> ScreenshotStreamAsync()
    {
        return _page.ScreenshotStreamAsync();
    }

    public Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options)
    {
        return _page.ScreenshotStreamAsync(options);
    }

    public Task<string[]> SelectAsync(string selector, params string[] values)
    {
        return _page.SelectAsync(selector, values);
    }

    public Task SetBurstModeOffAsync()
    {
        return _page.SetBurstModeOffAsync();
    }

    public Task SetBypassCSPAsync(bool enabled)
    {
        return _page.SetBypassCSPAsync(enabled);
    }

    public Task SetCacheEnabledAsync(bool enabled = true)
    {
        return _page.SetCacheEnabledAsync(enabled);
    }

    public Task SetContentAsync(string html, NavigationOptions? options = null)
    {
        return _page.SetContentAsync(html, options);
    }

    public Task SetCookieAsync(params CookieParam[] cookies)
    {
        return _page.SetCookieAsync(cookies);
    }

    public Task SetDragInterceptionAsync(bool enabled)
    {
        return _page.SetDragInterceptionAsync(enabled);
    }

    public Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers)
    {
        return _page.SetExtraHttpHeadersAsync(headers);
    }

    public Task SetGeolocationAsync(GeolocationOption options)
    {
        return _page.SetGeolocationAsync(options);
    }

    public Task SetJavaScriptEnabledAsync(bool enabled)
    {
        return _page.SetJavaScriptEnabledAsync(enabled);
    }

    public Task SetOfflineModeAsync(bool value)
    {
        return _page.SetOfflineModeAsync(value);
    }

    public Task SetRequestInterceptionAsync(bool value)
    {
        return _page.SetRequestInterceptionAsync(value);
    }

    public Task SetUserAgentAsync(string userAgent, UserAgentMetadata? userAgentData = null)
    {
        return _page.SetUserAgentAsync(userAgent, userAgentData);
    }

    public Task SetViewportAsync(ViewPortOptions viewport)
    {
        return _page.SetViewportAsync(viewport);
    }

    public Task TapAsync(string selector)
    {
        return _page.TapAsync(selector);
    }

    public Task TypeAsync(string selector, string text, TypeOptions? options = null)
    {
        return _page.TypeAsync(selector, text, options);
    }

    public Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions? options = null)
    {
        return _page.WaitForExpressionAsync(script, options);
    }

    public Task<IFrame> WaitForFrameAsync(string url, WaitForOptions? options = null)
    {
        return _page.WaitForFrameAsync(url, options);
    }

    public Task<IFrame> WaitForFrameAsync(Func<IFrame, bool> predicate, WaitForOptions? options = null)
    {
        return _page.WaitForFrameAsync(predicate, options);
    }

    public Task<FileChooser> WaitForFileChooserAsync(WaitForOptions? options = null)
    {
        return _page.WaitForFileChooserAsync(options);
    }

    public Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions? options = null, params object[] args)
    {
        return _page.WaitForFunctionAsync(script, options, args);
    }

    public Task<IJSHandle> WaitForFunctionAsync(string script, params object[] args)
    {
        return _page.WaitForFunctionAsync(script, args);
    }

    public Task<IResponse> WaitForNavigationAsync(NavigationOptions? options = null)
    {
        return _page.WaitForNavigationAsync(options);
    }

    public Task WaitForNetworkIdleAsync(WaitForNetworkIdleOptions? options = null)
    {
        return _page.WaitForNetworkIdleAsync(options);
    }

    public Task<IRequest> WaitForRequestAsync(Func<IRequest, bool> predicate, WaitForOptions? options = null)
    {
        return _page.WaitForRequestAsync(predicate, options);
    }

    public Task<IRequest> WaitForRequestAsync(string url, WaitForOptions? options = null)
    {
        return _page.WaitForRequestAsync(url, options);
    }

    public Task<IResponse> WaitForResponseAsync(Func<IResponse, bool> predicate, WaitForOptions? options = null)
    {
        return _page.WaitForResponseAsync(predicate, options);
    }

    public Task<IResponse> WaitForResponseAsync(Func<IResponse, Task<bool>> predicate, WaitForOptions? options = null)
    {
        return _page.WaitForResponseAsync(predicate, options);
    }

    public Task<IResponse> WaitForResponseAsync(string url, WaitForOptions? options = null)
    {
        return _page.WaitForResponseAsync(url, options);
    }

    public Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions? options = null)
    {
        return _page.WaitForSelectorAsync(selector, options);
    }

    public Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions? options = null)
    {
        return _page.WaitForXPathAsync(xpath, options);
    }

    public Task<IElementHandle[]> XPathAsync(string expression)
    {
        return _page.XPathAsync(expression);
    }

    public Task<DeviceRequestPrompt> WaitForDevicePromptAsync(WaitForOptions? options = null)
    {
        return _page.WaitForDevicePromptAsync(options);
    }

    public void AddRequestInterceptor(Func<IRequest, Task> interceptionTask)
    {
        _page.AddRequestInterceptor(interceptionTask);
    }

    public void RemoveRequestInterceptor(Func<IRequest, Task> interceptionTask)
    {
        _page.RemoveRequestInterceptor(interceptionTask);
    }

    public Task<ICDPSession> CreateCDPSessionAsync()
    {
        return _page.CreateCDPSessionAsync();
    }

    public Task RemoveScriptToEvaluateOnNewDocumentAsync(string identifier)
    {
        return _page.RemoveScriptToEvaluateOnNewDocumentAsync(identifier);
    }

    public Task SetBypassServiceWorkerAsync(bool bypass)
    {
        return _page.SetBypassServiceWorkerAsync(bypass);
    }

    public IAccessibility Accessibility => _page.Accessibility;
    public IBrowser Browser => _page.Browser;
    public IBrowserContext BrowserContext => _page.BrowserContext;
    public ICDPSession Client => _page.Client;
    public ICoverage Coverage => _page.Coverage;

    public int DefaultNavigationTimeout
    {
        get => _page.DefaultNavigationTimeout;
        set => _page.DefaultNavigationTimeout = value;
    }
    public int DefaultTimeout
    {
        get => _page.DefaultTimeout;
        set => _page.DefaultNavigationTimeout = value;
    }

    public IFrame[] Frames => _page.Frames;
    public bool IsClosed => _page.IsClosed;
    public bool IsDragInterceptionEnabled => _page.IsDragInterceptionEnabled;
    public IKeyboard Keyboard => _page.Keyboard;
    public IFrame MainFrame => _page.MainFrame;
    public IMouse Mouse => _page.Mouse;
    public ITarget Target => _page.Target;
    public ITouchscreen Touchscreen => _page.Touchscreen;
    public ITracing Tracing => _page.Tracing;
    public string Url => _page.Url;
    public ViewPortOptions Viewport => _page.Viewport;
    public WebWorker[] Workers => _page.Workers;
    public bool IsJavaScriptEnabled => _page.IsJavaScriptEnabled;
    public bool IsServiceWorkerBypassed => _page.IsServiceWorkerBypassed;
    public event EventHandler? Close;
    public event EventHandler<ConsoleEventArgs>? Console;
    public event EventHandler<DialogEventArgs>? Dialog;
    public event EventHandler? DOMContentLoaded;
    public event EventHandler<ErrorEventArgs>? Error;
    public event EventHandler<FrameEventArgs>? FrameAttached;
    public event EventHandler<FrameEventArgs>? FrameDetached;
    public event EventHandler<FrameNavigatedEventArgs>? FrameNavigated;
    public event EventHandler? Load;
    public event EventHandler<MetricEventArgs>? Metrics;
    public event EventHandler<PageErrorEventArgs>? PageError;
    public event EventHandler<PopupEventArgs>? Popup;
    public event EventHandler<RequestEventArgs>? Request;
    public event EventHandler<RequestEventArgs>? RequestFailed;
    public event EventHandler<RequestEventArgs>? RequestFinished;
    public event EventHandler<RequestEventArgs>? RequestServedFromCache;
    public event EventHandler<ResponseCreatedEventArgs>? Response;
    public event EventHandler<WorkerEventArgs>? WorkerCreated;
    public event EventHandler<WorkerEventArgs>? WorkerDestroyed;

    private void SubscribeEvents()
    {
        _page.Close += (s, e) => Close?.Invoke(this, e);
        _page.Console += (s, e) => Console?.Invoke(this, e);
        _page.Dialog += (s, e) => Dialog?.Invoke(this, e);
        _page.DOMContentLoaded += (s, e) => DOMContentLoaded?.Invoke(this, e);
        _page.Error += (s, e) => Error?.Invoke(this, e);
        _page.FrameAttached += (s, e) => FrameAttached?.Invoke(this, e);
        _page.FrameDetached += (s, e) => FrameDetached?.Invoke(this, e);
        _page.FrameNavigated += (s, e) => FrameNavigated?.Invoke(this, e);
        _page.Load += (s, e) => Load?.Invoke(this, e);
        _page.Metrics += (s, e) => Metrics?.Invoke(this, e);
        _page.PageError += (s, e) => PageError?.Invoke(this, e);
        _page.Popup += (s, e) => Popup?.Invoke(this, e);
        _page.Request += (s, e) => Request?.Invoke(this, e);
        _page.RequestFailed += (s, e) => RequestFailed?.Invoke(this, e);
        _page.RequestFinished += (s, e) => RequestFinished?.Invoke(this, e);
        _page.RequestServedFromCache += (s, e) => RequestServedFromCache?.Invoke(this, e);
        _page.Response += (s, e) => Response?.Invoke(this, e);
        _page.WorkerCreated += (s, e) => WorkerCreated?.Invoke(this, e);
        _page.WorkerDestroyed += (s, e) => WorkerDestroyed?.Invoke(this, e);
    }

    private void UnsubscribeEvents()
    {
        _page.Close -= (s, e) => Close?.Invoke(this, e);
        _page.Console -= (s, e) => Console?.Invoke(this, e);
        _page.Dialog -= (s, e) => Dialog?.Invoke(this, e);
        _page.DOMContentLoaded -= (s, e) => DOMContentLoaded?.Invoke(this, e);
        _page.Error -= (s, e) => Error?.Invoke(this, e);
        _page.FrameAttached -= (s, e) => FrameAttached?.Invoke(this, e);
        _page.FrameDetached -= (s, e) => FrameDetached?.Invoke(this, e);
        _page.FrameNavigated -= (s, e) => FrameNavigated?.Invoke(this, e);
        _page.Load -= (s, e) => Load?.Invoke(this, e);
        _page.Metrics -= (s, e) => Metrics?.Invoke(this, e);
        _page.PageError -= (s, e) => PageError?.Invoke(this, e);
        _page.Popup -= (s, e) => Popup?.Invoke(this, e);
        _page.Request -= (s, e) => Request?.Invoke(this, e);
        _page.RequestFailed -= (s, e) => RequestFailed?.Invoke(this, e);
        _page.RequestFinished -= (s, e) => RequestFinished?.Invoke(this, e);
        _page.RequestServedFromCache -= (s, e) => RequestServedFromCache?.Invoke(this, e);
        _page.Response -= (s, e) => Response?.Invoke(this, e);
        _page.WorkerCreated -= (s, e) => WorkerCreated?.Invoke(this, e);
        _page.WorkerDestroyed -= (s, e) => WorkerDestroyed?.Invoke(this, e);
    }
}