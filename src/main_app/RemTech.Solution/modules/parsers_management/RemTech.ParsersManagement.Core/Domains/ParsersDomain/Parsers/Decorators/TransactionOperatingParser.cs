using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;

public sealed class TransactionOperatingParser<T>(
    IParsers parsers,
    ITransactionalParsers transactionalParsers
)
{
    private Func<IParsers, Task<Status<IParser>>>? _receivingMethod;
    private Func<Task<Status<T>>>? _logicMethod;
    private IMaybeParser? _maybeParser;

    public TransactionOperatingParser<T> WithReceivingMethod(
        Func<IParsers, Task<Status<IParser>>> receivingMethod
    )
    {
        _receivingMethod = receivingMethod;
        return this;
    }

    public TransactionOperatingParser<T> WithLogicMethod(Func<Task<Status<T>>> logicMethod)
    {
        _logicMethod = logicMethod;
        return this;
    }

    public TransactionOperatingParser<T> WithPutting(IMaybeParser maybe)
    {
        _maybeParser = maybe;
        return this;
    }

    public async Task<Status<T>> Process()
    {
        ArgumentNullException.ThrowIfNull(_receivingMethod);
        ArgumentNullException.ThrowIfNull(_logicMethod);
        ArgumentNullException.ThrowIfNull(_maybeParser);
        await using (parsers)
        {
            Status<IParser> fromDataSource = await _receivingMethod(parsers);
            if (fromDataSource.IsFailure)
                return fromDataSource.Error;
            await using ITransactionalParser transactional = await transactionalParsers.Add(
                fromDataSource.Value,
                CancellationToken.None
            );
            _maybeParser.Put(transactional);
            Status<T> status = await _logicMethod();
            if (status.IsFailure)
                return status.Error;
            Status commit = await transactional.Save(CancellationToken.None);
            if (commit.IsFailure)
                return commit.Error;
            return status.Value;
        }
    }
}
