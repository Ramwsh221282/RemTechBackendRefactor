namespace RemTech.SharedKernel.Core.PrimitivesModule.Immutable;

public sealed class ImmutableString(string value)
{
    private readonly string _value = value;

    public string Read() => _value;

    public override string ToString() => _value;

    public static implicit operator string(ImmutableString value)
    {
        return value.Read();
    }
}
