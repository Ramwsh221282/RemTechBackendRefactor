using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

public sealed class ParsedTransport(
    ParsedTransportIdentity identity,
    ParsedTransportPrice price,
    ParsedTransportText text,
    ParsedTransportPhotos photos
)
{
    private readonly ParsedTransportIdentity _identity = identity;
    private readonly ParsedTransportPrice _price = price;
    private readonly ParsedTransportText _text = text;
    private readonly ParsedTransportPhotos _photos = photos;

    public ParsedTransportIdentity Identify() => _identity;
}
