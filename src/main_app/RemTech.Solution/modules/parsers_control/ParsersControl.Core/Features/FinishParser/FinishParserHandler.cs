using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.FinishParser;

[TransactionalHandler]
public sealed class FinishParserHandler(ISubscribedParsersRepository repository)
    : ICommandHandler<FinishParserCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(
        FinishParserCommand command,
        CancellationToken ct = default
    )
    {
        Result<SubscribedParser> parser = await GetRequiredParser(command.Id, ct);
        Result<Unit> result = FinishParser(parser, command.TotalElapsedSeconds);
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

    private async Task<Result<SubscribedParser>> GetRequiredParser(
        Guid parserId,
        CancellationToken ct
    )
    {
        SubscribedParserQuery query = new(Id: parserId, WithLock: true);
        return await SubscribedParser.FromRepository(repository, query, ct);
    }

    private Result<Unit> FinishParser(Result<SubscribedParser> parser, long totalElapsedSeconds)
    {
        if (parser.IsFailure)
            return parser.Error;
        return parser.Value.FinishWork(totalElapsedSeconds);
    }
}
