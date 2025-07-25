using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;

public sealed class CharacteristicMeasure
{
    private readonly NotEmptyString _measure;

    public CharacteristicMeasure(NotEmptyString measure)
    {
        _measure = measure;
    }

    public CharacteristicMeasure(string? measure) : this(new NotEmptyString(measure))
    {
        
    }

    public CharacteristicMeasure()
    {
        _measure = new NotEmptyString(string.Empty);
    }

    public Characteristic Print(Characteristic origin)
    {
        return new Characteristic(origin, this);
    }

    public string Read() => _measure;
}