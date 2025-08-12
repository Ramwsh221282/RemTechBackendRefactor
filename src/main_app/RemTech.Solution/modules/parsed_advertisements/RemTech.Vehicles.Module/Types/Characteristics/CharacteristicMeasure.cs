using RemTech.Core.Shared.Primitives;

namespace RemTech.Vehicles.Module.Types.Characteristics;

internal sealed class CharacteristicMeasure
{
    private readonly NotEmptyString _measure;

    public CharacteristicMeasure(NotEmptyString measure)
    {
        _measure = measure;
    }

    public CharacteristicMeasure(string? measure)
        : this(new NotEmptyString(measure)) { }

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
