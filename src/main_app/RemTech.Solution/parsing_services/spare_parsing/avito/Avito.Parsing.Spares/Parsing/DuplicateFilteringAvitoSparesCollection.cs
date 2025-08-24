using Parsing.Grpc.Services.DuplicateIds;

namespace Avito.Parsing.Spares.Parsing;

internal sealed class DuplicateFilteringAvitoSparesCollection(
    GrpcDuplicateIdsClient client,
    IAvitoSparesCollection origin
) : IAvitoSparesCollection
{
    public async Task<IEnumerable<AvitoSpare>> Read()
    {
        AvitoSpare[] spares = (await origin.Read().ConfigureAwait(false)).ToArray();
        IEnumerable<string> duplicateIds = await client
            .GetDuplicateIdentifiers(spares.Select(s => s.Id()))
            .ConfigureAwait(false);
        HashSet<string> dupSet = new HashSet<string>(duplicateIds, StringComparer.Ordinal);
        return spares.DistinctBy(s => s.Id()).Where(s => !dupSet.Contains(s.Id()));
    }
}
