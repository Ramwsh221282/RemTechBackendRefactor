using ParsersControl.Core.Parsers.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.Parsers.Features.SetWorkTime;

public sealed class SetWorkTimeHandler(
    ITransactionSource transactionSource,
    ISubscribedParsersRepository repository
) : ICommandHandler<SetWorkTimeCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(
        SetWorkTimeCommand command, 
        CancellationToken ct = default)
    {
        ITransactionScope scope = await transactionSource.BeginTransaction(ct: ct);
        Result<ISubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<SubscribedParser> result = SetRequiredTime(command, parser);
        Result saving = await SaveChanges(parser, result, scope, ct);
        if (saving.IsFailure) return saving.Error;
        return result;
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser, 
        Result<SubscribedParser> result, 
        ITransactionScope scope,
        CancellationToken ct = default)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (result.IsFailure) return Result.Failure(result.Error);
        await repository.Save(parser.Value);
        return await scope.Commit(ct);
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(
        SetWorkTimeCommand command,
        CancellationToken ct = default)
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        Result<ISubscribedParser> parser = await repository.Get(query, ct: ct);
        return parser;
    }

    private Result<SubscribedParser> SetRequiredTime(SetWorkTimeCommand command, Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.AddWorkTime(command.TotalElapsedSeconds);
    }
}