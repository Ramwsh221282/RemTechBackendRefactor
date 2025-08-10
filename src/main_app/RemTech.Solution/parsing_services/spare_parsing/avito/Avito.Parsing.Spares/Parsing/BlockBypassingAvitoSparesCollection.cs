using Parsing.Avito.Common.BypassFirewall;

namespace Avito.Parsing.Spares.Parsing;

public sealed class BlockBypassingAvitoSparesCollection(
    IAvitoBypassFirewall bypass,
    IAvitoSparesCollection origin
) : IAvitoSparesCollection
{
    public async Task<IEnumerable<AvitoSpare>> Read()
    {
        if (!await bypass.Read())
            return [];
        return await origin.Read();
    }
}
