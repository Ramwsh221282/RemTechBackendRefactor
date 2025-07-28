using Serilog;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoBypassFirewallLogging(ILogger log, IAvitoBypassFirewall origin) : IAvitoBypassFirewall
{
    public async Task<bool> Read()
    {
        log.Information("Resolving avito firewall...");
        bool result = await origin.Read();
        if (result)
            log.Information("Firewall resolved: {0}.", result);
        else
            log.Information("Firewall resolved: {0}.", result);
        return result;
    }
}