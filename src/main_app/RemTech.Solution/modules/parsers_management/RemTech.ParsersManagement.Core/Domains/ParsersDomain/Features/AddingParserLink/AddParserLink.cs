using RemTech.ParsersManagement.Core.Common.Errors;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink;

public sealed class AddParserLink : IMaybeError
{
    private readonly MaybeBag<NotEmptyString> _linkName;
    private readonly MaybeBag<NotEmptyString> _linkUrl;
    private readonly IParser _parser;
    private readonly ErrorBag _error;

    public AddParserLink(IParser parser, string? linkName, string? linkUrl)
    {
        _parser = parser;
        Status<NotEmptyString> name = NotEmptyString.New(linkName);
        Status<NotEmptyString> url = NotEmptyString.New(linkUrl);
        _error = ErrorBag.New(name, url);
        _linkName = name;
        _linkUrl = url;
    }

    public NotEmptyString WhatName() => _linkName;

    public NotEmptyString WhatUrl() => _linkUrl;

    public bool Errored() => _error.Errored();

    public Error Error() => _error.Error();

    public IParser TakeOwner() => _parser;
}
