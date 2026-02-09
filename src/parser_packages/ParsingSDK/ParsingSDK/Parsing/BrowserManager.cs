using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;
using Serilog;

namespace ParsingSDK.Parsing;

public sealed class BrowserManager
{
    private const int MAX_CONCURRENT_PAGES = 1;
    
    private IBrowser? _browser;
    
    private readonly Channel<IPage> _pages;
    private ILogger Logger { get; }
    private ScrapingBrowserOptions External { get; }
    
    private static string _browserPath = string.Empty;
    

    public BrowserManager(IOptions<ScrapingBrowserOptions> options, ILogger logger)
    {
        Logger = logger.ForContext<BrowserManager>();
        External = options.Value;
        _browserPath = External.BrowserPath;
        _pages = Channel.CreateBounded<IPage>(new BoundedChannelOptions(MAX_CONCURRENT_PAGES)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });
    }

    internal void ReturnPage(IPage page)
    {
        Logger.Information("Возвращение страницы в пул.");
        if (page.IsClosed)
        {
            Logger.Warning("Страница закрыта.");
            page.Dispose();
            Logger.Warning("Страница удалена из пула.");
            return;
        }
        
        _pages.Writer.WriteAsync(page);
        Logger.Information("Страница возвращена в пул.");       
    }
    
    internal async Task ReturnPageAsync(IPage page)
    {
        Logger.Information("Возвращение страницы в пул.");
        if (page.IsClosed)
        {
            Logger.Warning("Страница закрыта.");
            await page.DisposeAsync();
            Logger.Warning("Страница удалена из пула.");
            return;
        }
        
        await _pages.Writer.WriteAsync(page);
        Logger.Information("Страница возвращена в пул.");
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
        IPage page = await _pages.Reader.ReadAsync();
        return new PageLease(this, page);
    }
    
    public async Task<IBrowser> ProvideBrowser()
    {
        return _browser ??= await InstantiateBrowser();
    }
    
    public async Task KillBrowserProcess()
    {
        if (_browser is null)
        {
            Logger.Warning("Браузер не создан. Невозможно завершить процесс.");
            return;
        }
        
        await KillPages();
        await _browser.DisposeAsync();
        _browser = null;
        string[] processNames = ["chrome", "chromium"];
        foreach (string processName in processNames)
        {
            Process?[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                continue;
            }
            
            Logger.Information("Завершение процесса {processName}", processName);
            KillProcesses(Process.GetProcessesByName(processName));
            Logger.Information("Завершен процесс {processName}", processName);
        }
    }

    private async Task KillPages()
    {
        Logger.Information("Завершение всех страниц.");
        IPage page = await _pages.Reader.ReadAsync();
        await page.DisposeAsync();
        Logger.Information("Завершено все страницы.");       
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
                _browserPath = GetCurrentBrowserPath();
            }
        }
        
        IBrowser browser =  await InstantiateBrowserWithPath(External);
        Logger.Information("Браузер создан.");
        await PopulatePages(browser);
        return browser;
    }

    private async Task PopulatePages(IBrowser browser)
    {
        for (int i = 0; i < MAX_CONCURRENT_PAGES; i++)
        {
            IPage page = await browser.NewPageAsync();
            await _pages.Writer.WriteAsync(page);
        }
        
        Logger.Information("Пулы страниц заполнены. Макс число пула: {MaxPoolCount}", MAX_CONCURRENT_PAGES);
    }

    private static void KillProcesses(IEnumerable<Process?> processes)
    {
        foreach (Process? process in processes)
        {
            if (process is null)
            {
                continue;
            }
            
            process.Kill();
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
            throw new FileNotFoundException($"Browser file not found: {path}");
        }
        
        Logger.Information("Browser path: {Path}. Browser exists.", path);
        LogAsDirectory();
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
}