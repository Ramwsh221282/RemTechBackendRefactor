using System.Diagnostics;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;
using Serilog;
using Serilog.Core;

namespace ParsingSDK.Parsing;

public sealed class BrowserFactory
{
    private ILogger Logger { get; }
    private ScrapingBrowserOptions External { get; }

    private static string _browserPath = string.Empty;

    public BrowserFactory(IOptions<ScrapingBrowserOptions> options, ILogger logger)
    {
        Logger = logger.ForContext<BrowserFactory>();
        External = options.Value;
        _browserPath = External.BrowserPath;
    }
    
    public async Task<IBrowser> ProvideBrowser()
    {
        LogBrowserOptions();
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
        return browser;
    }

    public static void KillBrowserProcess()
    {
        using Logger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();   
        string[] processNames = ["chrome", "chromium"];
        foreach (string processName in processNames)
        {
            Process?[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                continue;
            }
            
            logger.Information("Завершение процесса {processName}", processName);
            KillProcesses(Process.GetProcessesByName(processName));
            logger.Information("Завершен процесс {processName}", processName);
        }
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

    private static async Task<IBrowser> InstantiateBrowserWithPath(ScrapingBrowserOptions options)
    {
        ScrapingBrowserOptions withPath = new() { BrowserPath = _browserPath, Headless = options.Headless };
        
        string[] launchArgs = 
        [
            "--no-sandbox", 
            "--disable-gpu", 
            "--disable-dev-shm-usage", 
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
        BrowserFetcherOptions options = new() { Browser = SupportedBrowser.Chromium  };
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