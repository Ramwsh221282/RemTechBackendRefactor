namespace RemTech.SharedKernel.Core.PrimitivesModule.Immutable;

public sealed class ImmutableString
{
    private readonly string _value;

    public ImmutableString(string value)
    {
        _value = value;
    }

    public string Read()
    {
        return _value;
    }
    
    public override string ToString()
    {
        return _value;
    }

    public static implicit operator string(ImmutableString value)
    {
        return value.Read();
    }
}