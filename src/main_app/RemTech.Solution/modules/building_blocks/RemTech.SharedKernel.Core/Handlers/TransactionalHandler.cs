using System.Reflection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers.Attributes;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace RemTech.SharedKernel.Core.Handlers;

public sealed class TransactionalHandler<TCommand, TResult> 
    : ITransactionalCommandHandler<TCommand, TResult>
    where TCommand : ICommand
{
    private IEnumerable<IEventTransporter<TCommand, TResult>> Transporters { get; }
    private ICommandHandler<TCommand, TResult> Inner { get; }
    private ITransactionSource TransactionSource { get; }

    public TransactionalHandler(
        IEnumerable<IEventTransporter<TCommand, TResult>> transporters,
        ICommandHandler<TCommand, TResult> inner, 
        ITransactionSource transactionSource)
    {
        Transporters = transporters;
        Inner = inner;
        TransactionSource = transactionSource;
    }
        
    public async Task<Result<TResult>> Execute(TCommand command, CancellationToken ct = default)
    {
        if (!HasTransactionalAttribute()) return await Inner.Execute(command, ct);
        await using ITransactionScope scope = await TransactionSource.BeginTransaction(ct);
        
        Result<TResult> result = await Inner.Execute(command, ct);
        if (result.IsFailure) return result.Error;
        
        Result commit = await scope.Commit(ct);
        if (commit.IsFailure) return commit.Error;
        
        await PublishEvents(result, ct); // publishing in external services. E.G: rabbit mq or in memory message bus.
        
        return result;
    }

    private async Task PublishEvents(TResult result, CancellationToken ct)
    {
        if (!Transporters.Any()) return;
        foreach (IEventTransporter<TCommand, TResult> transporter in Transporters)
        {
            await transporter.Transport(result, ct);
        }
    }
    
    private bool HasTransactionalAttribute()
    {
        TransactionalHandlerAttribute? attr = Inner.GetType().GetCustomAttribute<TransactionalHandlerAttribute>();
        if (attr == null) return false;
        return true;
    }
}