using RemTech.ParsersManagement.Core.Common.Errors;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Async;

public sealed class AsyncAddNewParser : IMaybeError
{
    private readonly Status<Name> _parserName;
    private readonly Status<ParsingType> _allowedType;
    private readonly Status<NotEmptyString> _domainString;
    private readonly ErrorBag _error;

    public AsyncAddNewParser(string? name, string? type, string? domain)
    {
        Status<NotEmptyString> nameString = NotEmptyString.New(name);
        if (nameString.IsFailure)
        {
            _error = ErrorBag.New(nameString);
            _parserName = _error.Error();
            _allowedType = _error.Error();
            _domainString = _error.Error();
            return;
        }

        Status<NotEmptyString> typeString = NotEmptyString.New(type);
        if (typeString.IsFailure)
        {
            _error = ErrorBag.New(nameString);
            _parserName = _error.Error();
            _allowedType = _error.Error();
            _domainString = _error.Error();
            return;
        }

        _allowedType = ParsingType.New(typeString);
        if (_allowedType.IsFailure)
        {
            _error = ErrorBag.New(_allowedType);
            _parserName = _error.Error();
            _allowedType = _error.Error();
            _domainString = _error.Error();
            return;
        }

        _domainString = NotEmptyString.New(domain);
        if (_domainString.IsFailure)
        {
            _error = ErrorBag.New(_domainString);
            _parserName = _error.Error();
            _allowedType = _error.Error();
            _domainString = _error.Error();
            return;
        }

        _error = new ErrorBag();
        _parserName = new Name(nameString);
    }

    public Name WhatName() => _parserName;

    public ParsingType WhatType() => _allowedType;

    public NotEmptyString WhatDomain() => _domainString;

    public bool Errored() => _error.Errored();

    public Error Error() => _error.Error();
}
