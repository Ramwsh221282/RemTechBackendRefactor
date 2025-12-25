using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.Features.SetParsedAmount;

public sealed class SetParserAmountCommandHandler(
    ITransactionSource transactionSource,
    ISubscribedParsersRepository repository
) : ICommandHandler<SetParsedAmountCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(
        SetParsedAmountCommand command, 
        CancellationToken ct = default)
    {
        ITransactionScope scope = await transactionSource.BeginTransaction(ct);
        Result<ISubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<SubscribedParser> result = SetRequiredAmount(command, parser);
        Result saving = await SaveChanges(scope, parser, result, ct);
        if (saving.IsFailure) return saving.Error;
        return result;
    }

    private async Task<Result> SaveChanges(
        ITransactionScope scope, 
        Result<ISubscribedParser> parser, 
        Result<SubscribedParser> result,
        CancellationToken ct)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (result.IsFailure) return Result.Failure(result.Error);
        await repository.Save(parser.Value);
        return await scope.Commit(ct);
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(
        SetParsedAmountCommand command,
        CancellationToken ct = default)
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        Result<ISubscribedParser> parser = await repository.Get(query, ct: ct);
        return parser;
    }
    
    private Result<SubscribedParser> SetRequiredAmount(
        SetParsedAmountCommand command, 
        Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.AddParserAmount(command.Amount);
    }
}