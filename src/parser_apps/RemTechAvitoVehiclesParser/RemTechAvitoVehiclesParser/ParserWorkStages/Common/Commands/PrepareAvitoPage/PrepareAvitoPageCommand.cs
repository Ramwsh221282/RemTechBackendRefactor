using AvitoFirewallBypass;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.PrepareAvitoPage;

public sealed class PrepareAvitoPageCommand(IPage page, AvitoBypassFactory bypassFactory) : IPrepareAvitoPageCommand
{
    public async Task Handle()
    {
        IAvitoBypassFirewall bypass = bypassFactory.Create(page);
        if (!await bypass.Bypass()) throw new InvalidOperationException("Unable to bypass Avito firewall");
        await page.ScrollBottom();
    }
}