using ParsersControl.Core.Parsers.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.Parsers.Features.StopParserWork;

public sealed class StopParserWorkHandler(
    ITransactionSource transactionSource,
    ISubscribedParsersRepository repository
) : ICommandHandler<StopParserWorkCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(
        StopParserWorkCommand command, 
        CancellationToken ct = default)
    {
        ITransactionScope scope = await transactionSource.BeginTransaction(ct: ct);
        Result<ISubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<SubscribedParser> finished = FinishWork(parser);
        Result saving = await SaveChanges(parser, scope, repository, ct);
        if (saving.IsFailure) return saving.Error;
        return finished;
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser, 
        ITransactionScope scope, 
        ISubscribedParsersRepository repository, 
        CancellationToken ct = default)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        await repository.Save(parser.Value);
        return await scope.Commit(ct);
    }
    
    private Result<SubscribedParser> FinishWork(Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.FinishWork();
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(
        StopParserWorkCommand command,
        CancellationToken ct = default)
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        return await repository.Get(query, ct: ct);
    }
}