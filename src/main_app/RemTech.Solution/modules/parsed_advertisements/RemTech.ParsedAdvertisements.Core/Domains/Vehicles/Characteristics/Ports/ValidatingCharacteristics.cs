using RemTech.Core.Shared.Primitives;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports;

public sealed class ValidatingCharacteristics(ICharacteristics origin) : ICharacteristics
{
    public Status<CharacteristicEnvelope> Add(string? name)
    {
        NotEmptyString ctxName = new(name);
        if (!ctxName)
            return new ValidationError<CharacteristicEnvelope>(
                $"Некорректное название характеристики: {(string)ctxName}"
            );
        return origin.Add(name);
    }

    public MaybeBag<CharacteristicEnvelope> GetByName(string? name)
    {
        return origin.GetByName(name);
    }
}
