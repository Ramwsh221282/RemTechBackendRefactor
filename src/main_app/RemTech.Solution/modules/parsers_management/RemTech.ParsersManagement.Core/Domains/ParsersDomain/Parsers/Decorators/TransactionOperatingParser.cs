using System.Transactions;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;

public sealed class TransactionOperatingParser<T>(IParsers parsers)
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
        try
        {
            using TransactionScope txn = new(
                TransactionScopeOption.Required,
                new TransactionOptions(),
                TransactionScopeAsyncFlowOption.Enabled
            );
            Status<IParser> fromDataSource = await _receivingMethod(parsers);
            if (fromDataSource.IsFailure)
                return fromDataSource.Error;
            _maybeParser.Put(fromDataSource.Value);
            Status<T> status = await _logicMethod();
            txn.Complete();
            return status;
        }
        catch
        {
            return Error.Conflict("Ошибка транзакции.");
        }
    }
}
