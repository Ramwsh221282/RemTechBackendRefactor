using RemTech.ParsersManagement.Core.Common.Primitives;

namespace RemTech.ParsersManagement.Core.Common.ValueObjects;

public sealed class Name
{
    private readonly NotEmptyString _name;

    public Name(NotEmptyString name) => _name = name;

    public NotEmptyString NameString() => _name;

    public bool Same(Name other) => _name.Same(other._name);

    public static implicit operator Name(NotEmptyString input) => new(input);

    public static implicit operator string(Name name) => name._name;

    public override bool Equals(object? obj) =>
        obj switch
        {
            null => false,
            Name n => n._name.Equals(_name),
            _ => false,
        };

    public override int GetHashCode() => _name.GetHashCode();
}
