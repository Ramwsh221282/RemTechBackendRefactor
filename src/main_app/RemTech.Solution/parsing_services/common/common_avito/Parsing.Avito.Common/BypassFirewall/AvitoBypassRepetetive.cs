using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoBypassRepetetive(IPage page, IAvitoBypassFirewall origin, int repeatCount = 5) : IAvitoBypassFirewall
{
    public async Task<bool> Read()
    {
        for (int i = 0; i < repeatCount; i++)
        {
            if (await origin.Read())
                return true;

            await new PageReload(page).Do();
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        return false;
    }
}