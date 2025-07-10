using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports;

public interface ICharacteristics
{
    Status<CharacteristicEnvelope> Add(string? name);
    MaybeBag<CharacteristicEnvelope> GetByName(string? name);
}
