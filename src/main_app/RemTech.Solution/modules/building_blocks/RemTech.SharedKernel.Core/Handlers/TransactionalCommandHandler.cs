using System.Reflection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers.Attributes;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace RemTech.SharedKernel.Core.Handlers;

public sealed class TransactionalCommandHandler<TCommand, TResult> 
    : ITransactionalCommandHandler<TCommand, TResult>
    where TCommand : ICommand
{
    private ICommandHandler<TCommand, TResult> Inner { get; }
    private ITransactionSource TransactionSource { get; }

    public TransactionalCommandHandler(
        ICommandHandler<TCommand, TResult> inner, 
        ITransactionSource transactionSource)
    {
        Inner = inner;
        TransactionSource = transactionSource;
    }
        
    public async Task<Result<TResult>> Execute(TCommand command, CancellationToken ct = default)
    {
        if (!HasTransactionalAttribute()) return await Inner.Execute(command, ct);
        ITransactionScope scope = await TransactionSource.BeginTransaction(ct);
        Result<TResult> result = await Inner.Execute(command, ct);
        Result commit = await scope.Commit(ct);
        return commit.IsFailure ? commit.Error : result;
    }

    private bool HasTransactionalAttribute()
    {
        TransactionalHandlerAttribute? attr = Inner.GetType().GetCustomAttribute<TransactionalHandlerAttribute>();
        if (attr == null) return false;
        return true;
    }
}