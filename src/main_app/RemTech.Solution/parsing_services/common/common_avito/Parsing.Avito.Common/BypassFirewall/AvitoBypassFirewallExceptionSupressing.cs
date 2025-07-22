namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoBypassFirewallExceptionSupressing(IAvitoBypassFirewall origin) : IAvitoBypassFirewall
{
    public async  Task<bool> Read()
    {
        try
        {
            return await origin.Read();
        }
        catch
        {
            return false;
        }
    }
}