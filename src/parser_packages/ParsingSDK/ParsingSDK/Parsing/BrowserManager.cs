using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;
using Serilog;

namespace ParsingSDK.Parsing;

public sealed class BrowserManager : IDisposable, IAsyncDisposable
{
    private const int MAX_CONCURRENT_PAGES = 1;
    
    private static string _browserPath = string.Empty;
    
    private IBrowser? _browser;
    private Channel<IPage>? _pages;
    private ILogger Logger { get; }
    private ScrapingBrowserOptions External { get; }
    
    public BrowserManager(IOptions<ScrapingBrowserOptions> options, ILogger logger)
    {
        Logger = logger.ForContext<BrowserManager>();
        External = options.Value;
        _browserPath = External.BrowserPath;
    }

    public async Task<IPage> RecreatePage(IPage page)
    {
        await KillPage(page);
        _browser ??= await InstantiateBrowser();
        _pages ??= CreatePagesChannel();
        await PopulatePages(_browser, _pages);
        return await ProvidePage();
    }

    public void ReleasePageUsedMemoryResources()
    {
        CallFinalizersForMemoryClear();
    }
    
    public async Task<IBrowser> RecreateBrowser()
    {
        Logger.Information("Пересоздание браузера.");
        await KillBrowserProcess();
        IBrowser browser = await InstantiateBrowser();
        Logger.Information("Браузер пересоздан.");
        return browser;
    }
    
    public async Task<IPage> RecreatePages()
    {
        Logger.Information("Пересоздание страниц.");
        await KillPages();
        IPage page = await ProvidePage();
        Logger.Information("Страницы пересозданы.");
        return page;
    }
    
    public async Task<IPage> ProvidePage()
    {
        _browser ??= await InstantiateBrowser();
        IPage page = await InstantiatePagesChannel().Reader.ReadAsync();
        return new PageLease(this, page);
    }
    
    public async Task<IBrowser> ProvideBrowser()
    {
        _browser ??= await InstantiateBrowser();
        return _browser;
    }
    
    public void Dispose()
    {
        if (_browser is not null)
        {
            KillBrowserProcess().Wait();   
        }

        if (_pages is not null)
        {
            CloseChannel().Wait();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await KillBrowserProcess();    
        }

        if (_pages is not null)
        {
            await CloseChannel();
        }
    }
    
    public static void ForceKillBrowserProcess()
    {
        Process[] processes = [..Process.GetProcessesByName("chrome"), ..Process.GetProcessesByName("chromium")];
        foreach (Process process in processes)
        {
            using (process)
            {
                process.Kill();
            }
        }        

        CallFinalizersForMemoryClear();
    }

    private async Task KillBrowserProcess()
    {
        if (_browser is null)
        {
            Logger.Warning("Браузер не создан. Невозможно завершить процесс.");
            return;
        }
        
        Process process = _browser.Process;
        string name = process.ProcessName;
        Logger.Information("Завершение процесса браузера {Name}", name);
        LogProcessMemory(Logger);
        
        await KillPages();
        await _browser.CloseAsync();
        await _browser.DisposeAsync();
        KillBrowserProcess(process);
        
        _browser = null;
        LogProcessMemory(Logger);
        await CloseChannel();
        CallFinalizersForMemoryClear();
        Logger.Information("Завершен процесс браузера {Name}", name);
    }

    private async Task CloseChannel()
    {
        if (_pages is not null)
        {
            _pages.Writer.TryComplete();
            await _pages.Reader.Completion;
            _pages = null;
        }
    }

    private static Channel<IPage> CreatePagesChannel()
    {
        return Channel.CreateBounded<IPage>(new BoundedChannelOptions(MAX_CONCURRENT_PAGES)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });
    }
    
    private static void LogProcessMemory(ILogger logger)
    {
        Process process = Process.GetCurrentProcess();
        long managed = GC.GetTotalMemory(false) / 1024 / 1024;
        long workingSet = process.WorkingSet64 / 1024 / 1024;
        long privateMem = process.PrivateMemorySize64 / 1024 / 1024;

        logger.Information("Управляемая память: {managed} MB", managed);
        logger.Information("Память закрепленная за процессом: {workingSet} MB", workingSet);
        logger.Information("Память принадлежащая текущему процессу: {privateMem} MB", privateMem);
    }
    
    private async Task KillPages()
    {
        if (_pages is null)
        {
            return;
        }
        
        Logger.Information("Завершение всех страниц.");
        while (_pages.Reader.TryRead(out var page)) 
        {
            if (page is null)
            {
                continue;
            }
            
            await KillPage(page);
        }
    }

    private static async Task KillPage(IPage page)
    {
        if (page is not null)
        {
            await page.DeleteCookieAsync();
            await page.CloseAsync();
            await page.DisposeAsync();
        }
    }

    private async Task<IBrowser> InstantiateBrowser()
    {
        LogBrowserOptions();
        LogBrowserPath();
        if (string.IsNullOrWhiteSpace(_browserPath))
        {
            Logger.Warning("Путь к браузеру пустой.");
            if (!BrowserExists())
            {
                Logger.Information("Загружается Chromium браузер.");
                await DownloadBrowser();
                Logger.Information("Chromium браузер загружен.");
            }
            
            _browserPath = GetCurrentBrowserPath();
        }
        
        ThrowIfBrowserFileDoesNotExist(_browserPath);
        IBrowser browser =  await InstantiateBrowserWithPath(External);
        Logger.Information("Браузер создан.");
        await PopulatePages(browser, InstantiatePagesChannel());
        Logger.Information("Страницы созданы.");       
        return browser;
    }

    private static async Task PopulatePages(IBrowser browser, Channel<IPage> pages)
    {
        for (int i = 0; i < MAX_CONCURRENT_PAGES; i++)
        {
            IPage page = await browser.NewPageAsync();
            await pages.Writer.WriteAsync(page);
        }
    }
    
    private void LogBrowserOptions()
    {
        Logger.Information("""
                           Настройки запуска браузера:
                           Путь к браузеру: {Path} 
                           Безголовый: {Headless}
                           """, External.BrowserPath, External.Headless);
    }

    private void LogBrowserPath()
    {
        string path = External.BrowserPath;
        bool fileExists = File.Exists(path);
        if (!fileExists)
        {
            Logger.Warning("Browser path: {Path}. Browser does not exist.", path);
            LogAsDirectory();
        }
        
        Logger.Information("Browser path: {Path}. Browser exists.", path);
        LogAsDirectory();
    }

    private static void ThrowIfBrowserFileDoesNotExist(string path)
    {
        if (!File.Exists(path))
        {
            throw new Exception($"Browser file does not exist: {path}");
        }
    }

    private void LogAsDirectory()
    {
        string path = External.BrowserPath;
        bool isDirectory = Directory.Exists(path);
        if (isDirectory)
        {
            Logger.Information("Path is directory.");
            string[] directoryData = Directory.GetDirectories(path);
            Logger.Information("Directory sub folders count: {Count}", directoryData.Length);
            foreach (string dirPath in directoryData)
            {
                Logger.Information("Subdir path: {Path}", dirPath);
            }
        }
    }

    private static void KillBrowserProcess(Process process)
    {
        if (process is not null)
        {
            using (process)
            {
                process.Kill(entireProcessTree: true);
            }
        }
    }

    private static void CallFinalizersForMemoryClear()
    {
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, blocking: true, compacting: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, blocking: true, compacting: true);
    }
    
    private static async Task<IBrowser> InstantiateBrowserWithPath(ScrapingBrowserOptions options)
    {
        ScrapingBrowserOptions withPath = new() { BrowserPath = _browserPath, Headless = options.Headless };
        
        string[] launchArgs = 
        [
            "--no-sandbox",
            "--disable-setuid-sandbox",
            "--disable-gpu",
            "--disable-extensions",
            "--disable-dev-shm-usage",
            "--no-zygote",
            "--no-first-run",
            "--disable-sync",
            "--disable-accelerated-2d-canvas",
            "--force-color-profile=srgb",
            "--renderer-process-limit=1",
            "--js-flags=\"--max-old-space-size=128\"",
            "--disk-cache-size=1",
            "--media-cache-size=1",
            "--disable-background-timer-throttling",
            "--disable-features=TranslateUI,ImprovedCookieControls,",
            "AudioServiceOutOfProcess,SitePerProcess"
        ];
        
        LaunchOptions launchOptions = new()
        {
            Headless = withPath.Headless, 
            ExecutablePath = withPath.BrowserPath, 
            Args = launchArgs
        };

        return await Puppeteer.LaunchAsync(launchOptions);
    }
    
    private async Task DownloadBrowser() 
    {
        BrowserFetcherOptions options = new() { Browser = SupportedBrowser.Chrome  };
        BrowserFetcher fetcher = new(options);
        await fetcher.DownloadAsync();
    }

    private string GetCurrentBrowserPath()
    {
        BrowserFetcher fetcher = new();
        InstalledBrowser[] browsers = fetcher.GetInstalledBrowsers().ToArray();
        return browsers[0].GetExecutablePath();
    }
    
    private bool BrowserExists()
    {
        BrowserFetcher fetcher = new();
        InstalledBrowser[] browsers = fetcher.GetInstalledBrowsers().ToArray();
        return browsers.Length != 0;
    }
    internal void ReturnPage(IPage page)
    {
        Logger.Information("Возвращение страницы в пул.");
        if (page.IsClosed)
        {
            Logger.Warning("Страница закрыта.");
            KillPage(page).Wait();
            Logger.Warning("Страница удалена из пула.");
            return;
        }

        Channel<IPage> pages = InstantiatePagesChannel();
        pages.Writer.WriteAsync(page);
        Logger.Information("Страница возвращена в пул.");       
    }
    
    internal async Task ReturnPageAsync(IPage page)
    {
        Logger.Information("Возвращение страницы в пул.");
        if (page.IsClosed)
        {
            Logger.Warning("Страница закрыта.");
            await KillPage(page);
            Logger.Warning("Страница удалена из пула.");
            return;
        }
        
        Channel<IPage> pages = InstantiatePagesChannel();
        await pages.Writer.WriteAsync(page);
        Logger.Information("Страница возвращена в пул.");
    }

    private Channel<IPage> InstantiatePagesChannel()
    {
        _pages ??= CreatePagesChannel();
        return _pages;
    }
}