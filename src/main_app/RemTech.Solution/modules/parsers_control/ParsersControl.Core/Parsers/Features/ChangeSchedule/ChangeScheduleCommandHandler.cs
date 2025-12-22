using ParsersControl.Core.Parsers.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.Parsers.Features.ChangeSchedule;

public sealed class ChangeScheduleCommandHandler(
    ITransactionSource transactionSource,
    ISubscribedParsersRepository repository
) : ICommandHandler<ChangeScheduleCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(ChangeScheduleCommand command)
    {
        ITransactionScope scope = await transactionSource.BeginTransaction();
        Result<ISubscribedParser> parser = await GetRequiredParser(command);
        Result<SubscribedParser> result = SetRequiredSchedule(command, parser);
        Result saving = await SaveChanges(scope, parser, result);
        if (saving.IsFailure) return saving.Error;
        return result; 
    }

    private async Task<Result> SaveChanges(ITransactionScope scope, Result<ISubscribedParser> parser, Result<SubscribedParser> result)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (result.IsFailure) return Result.Failure(result.Error);
        await repository.Save(parser.Value);
        Result commit = await scope.Commit();
        return commit;
    }
    
    private Result<SubscribedParser> SetRequiredSchedule(ChangeScheduleCommand command, Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        Result<SubscribedParser> result = parser.Value.ChangeScheduleWaitDays(command.WaitDays);
        return result;
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(ChangeScheduleCommand command)
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        Result<ISubscribedParser> parser = await repository.Get(query);
        return parser;
    }
}