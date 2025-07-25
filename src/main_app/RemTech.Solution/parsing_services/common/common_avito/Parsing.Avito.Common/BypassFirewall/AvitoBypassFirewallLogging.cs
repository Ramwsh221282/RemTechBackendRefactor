using RemTech.Logging.Library;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoBypassFirewallLogging(ICustomLogger log, IAvitoBypassFirewall origin) : IAvitoBypassFirewall
{
    public async Task<bool> Read()
    {
        log.Info("Resolving avito firewall...");
        bool result = await origin.Read();
        if (result)
            log.Info("Firewall resolved: {0}.", result);
        else
            log.Warn("Firewall resolved: {0}.", result);
        return result;
    }
}