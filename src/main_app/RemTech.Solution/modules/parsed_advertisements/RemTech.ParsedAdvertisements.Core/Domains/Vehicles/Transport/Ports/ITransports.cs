namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Ports;

public interface ITransports
{
    Task<ParsedTransport> Add(ParsedTransport transport, CancellationToken ct = default);
}
