using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.StopParserWork;

[TransactionalHandler]    
public sealed class StopParserWorkHandler(ISubscribedParsersRepository repository) : ICommandHandler<StopParserWorkCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(StopParserWorkCommand command, CancellationToken ct = default)
    {
        Result<SubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<Unit> finished = FinishWork(parser);
        return await SaveChanges(parser, finished, ct).Map(() => parser.Value);
    }

    private async Task<Result> SaveChanges(Result<SubscribedParser> parser, Result<Unit> finished, CancellationToken ct)
    {
        if (finished.IsFailure) return Result.Failure(finished.Error);
        if (parser.IsFailure) return Result.Failure(parser.Error);
        await parser.Value.SaveChanges(repository, ct);
        return Result.Success();
    }
    
    private Result<Unit> FinishWork(Result<SubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.FinishWork();
    }
    
    private async Task<Result<SubscribedParser>> GetRequiredParser(StopParserWorkCommand command, CancellationToken ct = default)
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        return await SubscribedParser.FromRepository(repository, query, ct);
    }
}