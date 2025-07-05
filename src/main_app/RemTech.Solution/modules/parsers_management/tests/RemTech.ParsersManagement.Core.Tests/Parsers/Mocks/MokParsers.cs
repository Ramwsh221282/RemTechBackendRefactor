using RemTech.ParsersManagement.Core.Common.Extensions;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Mocks;

public sealed class MokParsers : IParsers
{
    private Dictionary<NotEmptyGuid, IParser> Parsers { get; } = [];

    public Task<Status<IParser>> Find(Name name, CancellationToken ct = default)
    {
        MaybeBag<IParser> parser = Parsers.Values.Maybe(p => p.Identification().SameBy(name));
        Status<IParser> status = parser.Any()
            ? Status<IParser>.Success(parser.Take())
            : new ParserWithNameNotFoundError(name).Read();
        return Task.FromResult(status);
    }

    public Task<Status<IParser>> Find(NotEmptyGuid id, CancellationToken ct = default)
    {
        MaybeBag<IParser> parser = Parsers.Values.Maybe(p => p.Identification().SameBy(id));
        Status<IParser> status = parser.Any()
            ? Status<IParser>.Success(parser.Take())
            : new ParserWithIdNotFoundError(id).Read();
        return Task.FromResult(status);
    }

    public Task<Status<IParser>> Find(
        ParsingType type,
        NotEmptyString domain,
        CancellationToken ct = default
    )
    {
        MaybeBag<IParser> parser = Parsers.Values.Maybe(p =>
            p.Identification().SameBy(type) && p.Domain().SameBy(domain)
        );
        Status<IParser> status = parser.Any()
            ? Status<IParser>.Success(parser.Take())
            : new ParserWithTypeAndDomainNotFoundError(type, domain);
        return Task.FromResult(status);
    }

    public Task<Status> Add(IParser parser, CancellationToken ct = default)
    {
        if (ContainsParser(parser))
            return Task.FromResult(
                Status.Failure(new Error("Парсер уже существует.", ErrorCodes.Conflict))
            );
        Parsers.Add(parser.Identification().ReadId(), parser);
        return Task.FromResult(Status.Success());
    }

    public void SaveParser(IParser parser) => Parsers[parser.Identification().ReadId()] = parser;

    public bool ContainsParser(IParser parser)
    {
        if (Parsers.ContainsKey(parser.Identification().ReadId()))
            return true;
        Name parserName = parser.Identification().ReadName();
        if (Parsers.Values.Any(p => p.Identification().SameBy(parserName)))
            return true;
        ParserServiceDomain parserDomain = parser.Domain();
        ParsingType parserType = parser.Identification().ReadType();
        if (
            Parsers.Values.Any(p =>
                p.Domain().SameBy(parserDomain) && p.Identification().SameBy(parserType)
            )
        )
            return true;
        return false;
    }
}
