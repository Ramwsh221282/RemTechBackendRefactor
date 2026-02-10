using PuppeteerSharp;

namespace ParsingSDK.Parsing;

public static class PageActions
{
    extension(IPage page)
    {
        public async Task<Maybe<IElementHandle>> GetElementRetriable(string selectorQuery, int retryAmount = 30)
        {
            int currentAttempt = 0;
            while (currentAttempt < retryAmount)
            {
                Maybe<IElementHandle> maybe = await Maybe<IElementHandle>.Resolve(() => page.QuerySelectorAsync(selectorQuery));
                if (maybe.HasValue) return maybe;
                await Task.Delay(TimeSpan.FromSeconds(1));
                currentAttempt++;
            }
                
            return Maybe<IElementHandle>.None();
        }

        public async Task<IElementHandle[]> GetElementsRetriable(string selectorQuery, int retryAmount = 30)
        {
            int currentAttempt = 0;
            while (currentAttempt < retryAmount)
            {
                IElementHandle[] elements = await page.QuerySelectorAllAsync(selectorQuery);
                if (elements.Length > 0) return elements;
                await Task.Delay(TimeSpan.FromSeconds(1));
                currentAttempt++;
            }

            return [];
        }
        
        public async Task ScrollBottom()
        {
            await page.EvaluateExpressionAsync("window.scrollBy(0, document.documentElement.scrollHeight)");
        }

        public async Task ScrollTop()
        {
            await page.EvaluateExpressionAsync("window.scrollTo(0, 0)");
        }
        
        public async Task PerformQuickNavigation(string url, int timeout = 3000)
        {
            Task<IResponse> navigationTask = page.WaitForNavigationAsync(_navigationOptions);
            page.GoToAsync(url, _navigationOptions);
            try
            {
                await navigationTask;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Puppeteer navigation error: {ex.Message}");
            }
        }

        public async Task<Maybe<IElementHandle>> ResilientWaitForSelectorWithReturn(string selector, int attempts = 5)
        {            
            int currentAttempt = 0;
            while (currentAttempt < attempts)
            {
                IElementHandle? maybe = await page.WaitForSelectorAsync(selector, _waitForSelectorOptions);
                if (maybe != null) return Maybe<IElementHandle>.Some(maybe);
                currentAttempt++;
            }
            return Maybe<IElementHandle>.None();
        }
        
        public async Task ResilientWaitForSelector(string selector, int attempts = 5)
        {            
            int currentAttempt = 0;
            while (currentAttempt < attempts)
            {
                try
                { 
                    await page.WaitForSelectorAsync(selector, _waitForSelectorOptions);
                    break;
                }
                catch
                {
                    currentAttempt++;
                }
            }
        }
        
        public async Task PerformNavigation(string url)
        {
            int timeoutNavigation = 4000;
            try
            {
                await page.GoToAsync(url, _navigationOptions);
            }
            catch(NavigationException)
            {
                Console.WriteLine($"Puppeteer timeout navigation {timeoutNavigation} exceeded.");
            }
        }
    }

    private static readonly WaitForSelectorOptions _waitForSelectorOptions = new WaitForSelectorOptions()
    {
        Timeout = 5000
    };
    
    private static readonly NavigationOptions _navigationOptions = new NavigationOptions()
    {
        WaitUntil = [WaitUntilNavigation.Load],
        Timeout = 30000
    };
}