namespace Vehicles.Domain.LocationContext.ValueObjects;

public sealed record LocationAddress(IReadOnlyList<LocationAddressPart> Parts);
