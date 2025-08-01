﻿using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

public sealed class ParserServiceDomain
{
    private readonly Name _domainName;

    public ParserServiceDomain(NotEmptyString domainName) => _domainName = new Name(domainName);

    public Name Read() => _domainName;

    public bool SameBy(Name other) => _domainName.Same(other);

    public bool SameBy(IParser other) => other.Domain().SameBy(_domainName);

    public static implicit operator Name(ParserServiceDomain domain) => domain._domainName;

    public static implicit operator NotEmptyString(ParserServiceDomain domain) =>
        domain._domainName.NameString();

    public static implicit operator string(ParserServiceDomain domain) =>
        domain._domainName.NameString().StringValue();

    public override bool Equals(object? obj) =>
        obj switch
        {
            null => false,
            ParserServiceDomain psd => psd._domainName.Equals(_domainName),
            _ => false,
        };

    public override int GetHashCode() => _domainName.GetHashCode();
}
