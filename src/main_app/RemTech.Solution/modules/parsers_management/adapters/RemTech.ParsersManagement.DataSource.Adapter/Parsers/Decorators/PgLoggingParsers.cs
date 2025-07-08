using RemTech.Core.Shared.Functional;
using RemTech.Core.Shared.Primitives;
using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.Decorators;

public sealed class PgLoggingParsers(ICustomLogger logger, IParsers origin) : IParsers
{
    private const string Context = "PG Parsers";

    public void Dispose()
    {
        origin.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return origin.DisposeAsync();
    }

    public void OnParserFound(Status<IParser> parser)
    {
        if (parser.IsSuccess)
            logger.Info("{0}. Парсер найден: {1}.", Context, true);
    }

    public void OnParserNotFound(Status<IParser> parser)
    {
        if (parser.IsFailure)
            logger.Warn("{0}. Парсер найден: {1}.", Context, false);
    }

    public void OnParserAdded(Status status)
    {
        if (status.IsSuccess)
            logger.Info("{0}. Парсер добавлен: {1}.", Context, true);
    }

    public void OnParserNotAdded(Status status)
    {
        if (status.IsFailure)
            logger.Warn("{0}. Парсер добавлен: {1}.", Context, false);
    }

    public bool ParserAdded(Status status)
    {
        return status.IsSuccess;
    }

    public bool ParserNotAdded(Status status)
    {
        return status.IsFailure;
    }

    public bool ParserFound(Status<IParser> parser)
    {
        return parser.IsSuccess;
    }

    public bool ParserNotFound(Status<IParser> parser)
    {
        return parser.IsFailure;
    }

    public void ParserDiscoveryLog(Status<IParser> parser)
    {
        OnParserFound(parser);
        OnParserNotFound(parser);
    }

    public async Task<Status<IParser>> Find(Name name, CancellationToken ct = default)
    {
        string nameString = name;
        logger.Info("{0}. Получение парсера с названием: {1}.", Context, nameString);
        return new ActionPipeline<Status<IParser>>(await origin.Find(name, ct))
            .With(ParserFound, ParserDiscoveryLog)
            .With(ParserNotFound, ParserDiscoveryLog)
            .Process();
    }

    public async Task<Status<IParser>> Find(NotEmptyGuid id, CancellationToken ct = default)
    {
        Guid logId = id;
        logger.Info("{0}. Получение парсера с ID: {1}.", Context, logId);
        return new ActionPipeline<Status<IParser>>(await origin.Find(id, ct))
            .With(ParserFound, ParserDiscoveryLog)
            .With(ParserNotFound, ParserDiscoveryLog)
            .Process();
    }

    public async Task<Status<IParser>> Find(
        ParsingType type,
        NotEmptyString domain,
        CancellationToken ct = default
    )
    {
        string logType = type.Read();
        string logDomain = domain;
        logger.Info(
            "{0}. Получение парсера с типом: {1} и доменом: {2}.",
            Context,
            logType,
            logDomain
        );
        return new ActionPipeline<Status<IParser>>(await origin.Find(type, domain, ct))
            .With(ParserFound, ParserDiscoveryLog)
            .With(ParserNotFound, ParserDiscoveryLog)
            .Process();
    }

    public async Task<Status> Add(IParser parser, CancellationToken ct = default)
    {
        Guid logId = parser.Identification().ReadId();
        string logName = parser.Identification().ReadName();
        string logType = parser.Identification().ReadType().Read();
        string logDomain = parser.Domain();
        logger.Info(
            "{0}. Добавление парсера с ID: {1}, названием: {2}, типом: {3} доменом: {4}.",
            Context,
            logId,
            logName,
            logType,
            logDomain
        );
        return new ActionPipeline<Status>(await origin.Add(parser, ct))
            .With(ParserAdded, OnParserAdded)
            .With(ParserNotAdded, OnParserNotAdded)
            .Process();
    }
}
