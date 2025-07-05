using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

public sealed class ParserIdentity
{
    private readonly NotEmptyGuid _id;
    private readonly Name _name;
    private readonly ParsingType _type;
    private readonly ParserServiceDomain _domain;

    public ParserIdentity(Name name, ParsingType type, ParserServiceDomain domain)
    {
        _id = NotEmptyGuid.New();
        _name = name;
        _type = type;
        _domain = domain;
    }

    public ParserIdentity(NotEmptyGuid id, Name name, ParsingType type, ParserServiceDomain domain)
    {
        _id = id;
        _name = name;
        _type = type;
        _domain = domain;
    }

    public NotEmptyGuid ReadId() => _id;

    public Name ReadName() => _name;

    public ParsingType ReadType() => _type;

    public ParserServiceDomain Domain() => _domain;

    public bool SameBy(NotEmptyGuid id) => _id.Same(id);

    public bool SameBy(Name name) => _name.Same(name);

    public bool SameBy(ParsingType type) => _type.Read().Same(type.Read());

    public bool SameBy(ParserIdentity other) =>
        SameBy(other._id) && SameBy(other._name) && SameBy(other._type);
}
