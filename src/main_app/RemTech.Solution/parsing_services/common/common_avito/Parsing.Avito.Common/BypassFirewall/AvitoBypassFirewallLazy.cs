using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoBypassFirewallLazy(IPage page, IAvitoBypassFirewall origin)
    : IAvitoBypassFirewall
{
    private const string Title = ".firewall-title";

    public async Task<bool> Read()
    {
        if (await FirewallDoesNotPersist() == false)
            return true;

        await origin.Read();
        return !await FirewallDoesNotPersist();
    }

    private async Task<bool> FirewallDoesNotPersist()
    {
        IElementHandle? fireWall = await new PageElementSource(page).Read(Title);
        return fireWall != null;
    }
}
