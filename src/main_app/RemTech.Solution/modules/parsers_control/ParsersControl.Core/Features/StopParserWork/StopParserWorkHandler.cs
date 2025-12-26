using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.StopParserWork;

[TransactionalHandler]    
public sealed class StopParserWorkHandler(
    ISubscribedParsersRepository repository
) : ICommandHandler<StopParserWorkCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(
        StopParserWorkCommand command, 
        CancellationToken ct = default)
    {
        Result<ISubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<SubscribedParser> finished = FinishWork(parser);
        Result saving = await SaveChanges(parser);
        if (saving.IsFailure) return saving.Error;
        return finished;
    }

    private async Task<Result> SaveChanges(Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        await repository.Save(parser.Value);
        return Result.Success();
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