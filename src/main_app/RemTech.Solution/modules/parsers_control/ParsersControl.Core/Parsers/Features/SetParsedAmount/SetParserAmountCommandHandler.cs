using ParsersControl.Core.Parsers.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.Parsers.Features.SetParsedAmount;

public sealed class SetParserAmountCommandHandler(
    ITransactionSource transactionSource,
    ISubscribedParsersRepository repository
) : ICommandHandler<SetParsedAmountCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(SetParsedAmountCommand command)
    {
        ITransactionScope scope = await transactionSource.BeginTransaction();
        Result<ISubscribedParser> parser = await GetRequiredParser(command);
        Result<SubscribedParser> result = SetRequiredAmount(command, parser);
        Result saving = await SaveChanges(scope, parser, result);
        if (saving.IsFailure) return saving.Error;
        return result;        
    }

    private async Task<Result> SaveChanges(ITransactionScope scope, Result<ISubscribedParser> parser, Result<SubscribedParser> result)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (result.IsFailure) return Result.Failure(result.Error);
        await repository.Save(parser.Value);
        return await scope.Commit();
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(SetParsedAmountCommand command)
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        Result<ISubscribedParser> parser = await repository.Get(query);
        return parser;
    }
    
    private Result<SubscribedParser> SetRequiredAmount(SetParsedAmountCommand command, Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.AddParserAmount(command.Amount);
    }
}