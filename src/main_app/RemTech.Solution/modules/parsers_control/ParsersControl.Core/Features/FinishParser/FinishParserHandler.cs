using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.FinishParser;

[TransactionalHandler]
public sealed class FinishParserHandler
(
    ISubscribedParsersRepository repository
)
    : ICommandHandler<FinishParserCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(FinishParserCommand command, CancellationToken ct = default)
    {
        Result<ISubscribedParser> parser = await GetRequiredParser(command.Id, ct);
        Result<SubscribedParser> result = FinishParser(parser, command.TotalElapsedSeconds);
        Result saveResult = await SaveChanges(parser, result);
        return saveResult.Map(() => result.Value);
    }

    private async Task<Result> SaveChanges(Result<ISubscribedParser> parser, Result<SubscribedParser> result)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (result.IsFailure) return Result.Failure(result.Error);
        await repository.Save(parser.Value);
        return Result.Success();
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(Guid parserId, CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: parserId, WithLock: true);
        return await repository.Get(query, ct);
    }

    private Result<SubscribedParser> FinishParser(Result<ISubscribedParser> parser, long totalElapsedSeconds)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.FinishWork(totalElapsedSeconds);
    }
}