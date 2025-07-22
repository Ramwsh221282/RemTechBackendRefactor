using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoBypassFirewallLazy (
    IPage page, 
    IAvitoBypassFirewall origin)
    : IAvitoBypassFirewall
{
    public async  Task<bool> Read()
    {
        if (await FirewallDoesNotPersist() == false)
            return true;

        await origin.Read();
        return !await FirewallDoesNotPersist();
    }

    private async Task<bool> FirewallDoesNotPersist()
    {
        string fireWallSelector = string.Intern(".firewall-title");
        IElementHandle? fireWall = await new PageElementSource(page).Read(fireWallSelector);
        return fireWall != null;
    }
}