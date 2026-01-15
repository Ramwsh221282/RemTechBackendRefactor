using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.ChangeSchedule;

[TransactionalHandler]
public sealed class ChangeScheduleCommandHandler(ISubscribedParsersRepository repository)
    : ICommandHandler<ChangeScheduleCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(
        ChangeScheduleCommand command,
        CancellationToken ct = default
    )
    {
        Result<SubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<Unit> result = SetRequiredSchedule(command, parser);
        Result saving = await SaveChanges(parser, result, ct);
        return saving.IsFailure ? saving.Error : parser.Value;
    }

    private async Task<Result> SaveChanges(
        Result<SubscribedParser> parser,
        Result<Unit> result,
        CancellationToken ct
    )
    {
        if (parser.IsFailure)
            return Result.Failure(parser.Error);
        if (result.IsFailure)
            return Result.Failure(result.Error);
        await parser.Value.SaveChanges(repository, ct);
        return Result.Success();
    }

    private Result<Unit> SetRequiredSchedule(
        ChangeScheduleCommand command,
        Result<SubscribedParser> parser
    )
    {
        if (parser.IsFailure)
            return parser.Error;
        return parser.Value.ChangeScheduleWaitDays(command.WaitDays);
    }

    private async Task<Result<SubscribedParser>> GetRequiredParser(
        ChangeScheduleCommand command,
        CancellationToken ct
    )
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        return await SubscribedParser.FromRepository(repository, query, ct);
    }
}
