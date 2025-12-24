using ParsersControl.Core.Parsers.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.Parsers.Features.StartParserWork;

public sealed class StartParserCommandHandler(
    ITransactionSource transactionSource,
    ISubscribedParsersRepository repository,
    IOnParserStartedListener listener
) : ICommandHandler<StartParserCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(StartParserCommand command, CancellationToken ct = default)
    {
        ITransactionScope scope = await transactionSource.BeginTransaction(ct: ct);
        Result<ISubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<SubscribedParser> starting = CallParserWorkInvocation(parser);
        Result saving = await SaveChanges(parser, starting, repository, scope, ct);
        if (saving.IsFailure) return saving.Error;
        await NotifyListener(saving, starting.Value);
        return starting;
    }

    private async Task NotifyListener(Result saveResult, SubscribedParser parser)
    {
        if (saveResult.IsFailure) return;
        await listener.Handle(parser);
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(StartParserCommand command, CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        Result<ISubscribedParser> parser = await repository.Get(query, ct: ct);
        return parser;
    }

    private Result<SubscribedParser> CallParserWorkInvocation(Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.StartWork();
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser, 
        Result<SubscribedParser> starting, 
        ISubscribedParsersRepository repository, 
        ITransactionScope scope,
        CancellationToken ct)
    {
        if (starting.IsFailure) return Result.Failure(starting.Error);
        if (parser.IsFailure) return Result.Failure(parser.Error);
        await repository.Save(parser.Value);
        return await scope.Commit(ct);
    }
}