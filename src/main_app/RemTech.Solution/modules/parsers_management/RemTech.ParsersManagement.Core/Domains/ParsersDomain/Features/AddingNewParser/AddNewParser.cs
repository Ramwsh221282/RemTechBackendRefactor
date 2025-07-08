using RemTech.Core.Shared.Functional;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser;

public sealed class AddNewParser : IMaybeError
{
    private readonly Status<Name> _parserName;
    private readonly Status<ParsingType> _allowedType;
    private readonly Status<ParserServiceDomain> _domain;
    private readonly ErrorBag _error;

    public AddNewParser(Name name, ParsingType type, ParserServiceDomain domain)
    {
        _parserName = name;
        _allowedType = type;
        _domain = domain;
        _error = new ErrorBag();
    }

    public AddNewParser(string? name, string? type, string? domain)
    {
        Status<NotEmptyString> nameString = NotEmptyString.New(name);
        if (nameString.IsFailure)
        {
            _error = ErrorBag.New(nameString);
            _parserName = _error.Error();
            _allowedType = _error.Error();
            _domain = _error.Error();
            return;
        }

        Status<NotEmptyString> typeString = NotEmptyString.New(type);
        if (typeString.IsFailure)
        {
            _error = ErrorBag.New(nameString);
            _parserName = _error.Error();
            _allowedType = _error.Error();
            _domain = _error.Error();
            return;
        }

        _allowedType = ParsingType.New(typeString);
        if (_allowedType.IsFailure)
        {
            _error = ErrorBag.New(_allowedType);
            _parserName = _error.Error();
            _allowedType = _error.Error();
            _domain = _error.Error();
            return;
        }

        Status<NotEmptyString> domainString = NotEmptyString.New(domain);
        if (domainString.IsFailure)
        {
            _error = ErrorBag.New(domainString);
            _parserName = _error.Error();
            _allowedType = _error.Error();
            _domain = _error.Error();
            return;
        }

        _error = new ErrorBag();
        _parserName = new Name(nameString);
        _domain = new ParserServiceDomain(domainString);
    }

    public Name WhatName() => _parserName.Value;

    public ParsingType WhatType() => _allowedType;

    public ParserServiceDomain WhatDomain() => _domain;

    public bool Errored() => _error.Errored();

    public Error Error() => _error.Error();
}
