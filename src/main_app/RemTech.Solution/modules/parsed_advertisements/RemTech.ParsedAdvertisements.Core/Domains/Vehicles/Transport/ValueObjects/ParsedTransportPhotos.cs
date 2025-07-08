namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

public sealed record ParsedTransportPhotos
{
    private readonly HashSet<ParsedTransportPhoto> _photos;

    public ParsedTransportPhotos(IEnumerable<ParsedTransportPhoto> photos)
    {
        _photos = new HashSet<ParsedTransportPhoto>(photos);
    }
}
