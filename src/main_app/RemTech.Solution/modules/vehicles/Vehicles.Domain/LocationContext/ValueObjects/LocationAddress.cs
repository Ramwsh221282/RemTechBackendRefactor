using RemTech.Result.Pattern;
using Vehicles.Domain.LocationContext.Errors;

namespace Vehicles.Domain.LocationContext.ValueObjects;

public sealed record LocationAddress
{
    public IReadOnlyList<LocationAddressPart> Parts { get; }

    private LocationAddress(IEnumerable<LocationAddressPart> parts) => Parts = [.. parts];

    public bool IsSameAs(Location location) => location.Address.Parts.SequenceEqual(Parts);

    public bool IsSameAs(LocationAddress other) => Parts.SequenceEqual(other.Parts);

    public static Result<LocationAddress> Create(IEnumerable<string> parts)
    {
        Result<LocationAddressPart>[] results = [.. parts.Select(LocationAddressPart.Create)];
        Result<LocationAddressPart>? failure = results.FirstOrDefault(r => r.IsFailure);
        return failure?.Error ?? Create(results.Where(r => r.IsSuccess).Select(r => r.Value));
    }

    public static Result<LocationAddress> Create(IEnumerable<LocationAddressPart> parts)
    {
        LocationAddressPart[] array = [.. parts];
        LocationAddressPart[] distict = array.Distinct().ToArray();
        if (array.Length != distict.Length)
        {
            IEnumerable<string> nonUniqueParts = array
                .Select(a => a.Value)
                .GroupBy(v => v)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            return new LocationAddressPartsAreNotUniqueError(nonUniqueParts);
        }

        return new LocationAddress(array);
    }
}
