using Parsing.Grpc.Services.DuplicateIds;

namespace Avito.Parsing.Spares.Parsing;

internal sealed class DuplicateFilteringAvitoSparesCollection(
    GrpcDuplicateIdsClient client,
    IAvitoSparesCollection origin
) : IAvitoSparesCollection
{
    public async Task<IEnumerable<AvitoSpare>> Read()
    {
        IEnumerable<AvitoSpare> spares = await origin.Read();
        if (spares == null)
        {
            throw new ApplicationException(
                $"{nameof(DuplicateFilteringAvitoSparesCollection)} spares collection was null"
            );
        }

        IEnumerable<string> spareIds = spares.Select(s => s.Id());
        if (spareIds == null)
        {
            throw new ApplicationException(
                $"{nameof(DuplicateFilteringAvitoSparesCollection)} spares ids collection was null"
            );
        }

        IEnumerable<string> existingIds = await client.GetDuplicateIdentifiers(spareIds);
        if (existingIds == null)
        {
            throw new ApplicationException(
                $"{nameof(DuplicateFilteringAvitoSparesCollection)} existing ids collection was null"
            );
        }

        HashSet<string> dupSet = new HashSet<string>(existingIds);
        if (dupSet == null)
        {
            throw new ApplicationException(
                $"{nameof(DuplicateFilteringAvitoSparesCollection)} dupset was null"
            );
        }

        List<AvitoSpare> results = [];
        foreach (AvitoSpare spare in spares)
        {
            string spareId = spare.Id();
            if (string.IsNullOrWhiteSpace(spareId))
            {
                Console.WriteLine("Spare id was null");
                continue;
            }
            if (dupSet.Contains(spareId))
                continue;
            results.Add(spare);
        }
        return results;

        // try
        // {
        //     AvitoSpare[] spares = (await origin.Read().ConfigureAwait(false)).ToArray();
        //     IEnumerable<string> duplicateIds = await client
        //         .GetDuplicateIdentifiers(spares.Select(s => s.Id()))
        //         .ConfigureAwait(false);
        //     HashSet<string> dupSet = new HashSet<string>(duplicateIds, StringComparer.Ordinal);
        //     return spares.DistinctBy(s => s.Id()).Where(s => !dupSet.Contains(s.Id()));
        // }
        // catch
        // {
        //     Console.WriteLine($"Exception at {nameof(DuplicateFilteringAvitoSparesCollection)}");
        //     throw;
        // }
    }
}
