using System.Collections.Concurrent;
using System.Reflection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

public sealed class TransactionalHandler<TCommand, TResult>
    : ITransactionalCommandHandler<TCommand, TResult>
    where TCommand : ICommand
{
    private IEnumerable<IEventTransporter<TCommand, TResult>> Transporters { get; }
    private ICommandHandler<TCommand, TResult> Inner { get; }
    private ITransactionSource TransactionSource { get; }
    private Serilog.ILogger Logger { get; }

    private static readonly ConcurrentDictionary<Type, bool> TransactionalAttributeCache = new();

    public TransactionalHandler(
        Serilog.ILogger logger,
        IEnumerable<IEventTransporter<TCommand, TResult>> transporters,
        ICommandHandler<TCommand, TResult> inner,
        ITransactionSource transactionSource
    )
    {
        Logger = logger.ForContext<TransactionalHandler<TCommand, TResult>>();
        Transporters = transporters;
        Inner = inner;
        TransactionSource = transactionSource;
    }

    public async Task<Result<TResult>> Execute(TCommand command, CancellationToken ct = default)
    {
        if (!HasTransactionalAttribute(Inner))
            return await Inner.Execute(command, ct);

        ITransactionScope scope = await TransactionSource.BeginTransaction(ct);
        try
        {
            Result<TResult> result = await Inner.Execute(command, ct);
            if (result.IsFailure)
                return result.Error;

            Result commit = await scope.Commit(ct);
            if (commit.IsFailure)
                return commit.Error;

            await PublishEvents(result, ct); // publishing in external services. E.G: rabbit mq or in memory message bus.
            return result;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error during transaction.");
            throw;
        }
        finally
        {
            await scope.DisposeAsync();
            Logger.Information("Transaction disposed.");
        }
    }

    private async Task PublishEvents(TResult result, CancellationToken ct)
    {
        if (!Transporters.Any())
            return;
        foreach (IEventTransporter<TCommand, TResult> transporter in Transporters)
        {
            await transporter.Transport(result, ct);
        }
    }

    private static bool HasTransactionalAttribute(object? instance)
    {
        if (instance is null)
            return false;

        Type rootType = instance.GetType();
        return TransactionalAttributeCache.GetOrAdd(
            rootType,
            static t =>
            {
                HashSet<Type> visited = new HashSet<Type>();
                return HasTransactionalAttributeForTypeRecursive(t, visited);
            }
        );
    }

    private static bool HasTransactionalAttributeForTypeRecursive(Type type, ISet<Type> visited)
    {
        if (!visited.Add(type))
            return false;
        if (type.GetCustomAttribute<TransactionalHandlerAttribute>() != null)
            return true;

        const BindingFlags flags =
            BindingFlags.Instance
            | BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.FlattenHierarchy;
        foreach (FieldInfo field in type.GetFields(flags))
        {
            Type fieldType = field.FieldType;
            if (fieldType.GetCustomAttribute<TransactionalHandlerAttribute>() != null)
                return true;
            if (HasTransactionalAttributeForTypeRecursive(fieldType, visited))
                return true;
        }

        foreach (PropertyInfo prop in type.GetProperties(flags))
        {
            if (prop.GetIndexParameters().Length > 0)
                continue;

            Type propType = prop.PropertyType;

            if (propType.GetCustomAttribute<TransactionalHandlerAttribute>() != null)
                return true;

            if (HasTransactionalAttributeForTypeRecursive(propType, visited))
                return true;
        }

        return false;
    }
}
