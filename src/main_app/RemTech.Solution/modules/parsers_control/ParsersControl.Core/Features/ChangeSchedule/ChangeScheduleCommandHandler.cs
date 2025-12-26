using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.ChangeSchedule;

[TransactionalHandler]
public sealed class ChangeScheduleCommandHandler(
    ISubscribedParsersRepository repository
) : ICommandHandler<ChangeScheduleCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(ChangeScheduleCommand command, CancellationToken ct = default)
    {
        Result<ISubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<SubscribedParser> result = SetRequiredSchedule(command, parser);
        return await SaveChanges(parser, result).Map(() => result.Value);
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser, 
        Result<SubscribedParser> result)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (result.IsFailure) return Result.Failure(result.Error);
        await repository.Save(parser.Value);
        return Result.Success();
    }
    
    private Result<SubscribedParser> SetRequiredSchedule(
        ChangeScheduleCommand command, 
        Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        Result<SubscribedParser> result = parser.Value.ChangeScheduleWaitDays(command.WaitDays);
        return result;
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(
        ChangeScheduleCommand command,
        CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        Result<ISubscribedParser> parser = await repository.Get(query, ct: ct);
        return parser;
    }
}