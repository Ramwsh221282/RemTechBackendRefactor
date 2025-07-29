using RemTech.Core.Shared.Primitives;

namespace RemTech.Vehicles.Module.Types.Kinds.ValueObjects;

public sealed record VehicleKindText
{
    private readonly NotEmptyString _text;

    public VehicleKindText(NotEmptyString text)
    {
        _text = text;
    }

    public VehicleKindText(string? text)
    {
        _text = new NotEmptyString(text);
    }

    public static implicit operator string(VehicleKindText text) => text._text;

    public static implicit operator NotEmptyString(VehicleKindText text) => text._text;

    public static implicit operator bool(VehicleKindText? text)
    {
        return text != null && text._text;
    }
}
