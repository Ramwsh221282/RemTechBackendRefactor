using Parsing.SDK.Logging;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoBypassFirewallLogging(IParsingLog log, IAvitoBypassFirewall origin) : IAvitoBypassFirewall
{
    public async Task<bool> Read()
    {
        log.Info("Resolving avito firewall...");
        bool result = await origin.Read();
        if (result)
            log.Info("Firewall resolved: {0}.", result);
        else
            log.Warning("Firewall resolved: {0}.", result);
        return result;
    }
}