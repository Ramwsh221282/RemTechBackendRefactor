using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.PermantlyStartManyParsing;

[TransactionalHandler]
public sealed class PermantlyStartManyParsingHandler(ISubscribedParsersCollectionRepository repository)
    : ICommandHandler<PermantlyStartManyParsingCommand, IEnumerable<SubscribedParser>>
{
    public async Task<Result<IEnumerable<SubscribedParser>>> Execute(
        PermantlyStartManyParsingCommand command,
        CancellationToken ct = default
    )
    {
        Result<SubscribedParsersCollection> parsers = await GetParsers(command.Identifiers, ct);
        Result<Unit> result = PermanentlyStartParsers(parsers);
        Result<Unit> saving = await SaveChanges(result, parsers, ct);
        return saving.IsFailure ? saving.Error : Result.Success(parsers.Value.Read());
    }

    private async Task<Result<Unit>> SaveChanges(
        Result<Unit> enabling,
        Result<SubscribedParsersCollection> parsers,
        CancellationToken ct
    )
    {
        if (parsers.IsFailure)
            return parsers.Error;
        if (enabling.IsFailure)
            return enabling.Error;
        Result<Unit> saving = await repository.SaveChanges(parsers.Value, ct);
        return saving;
    }

    private static Result<Unit> PermanentlyStartParsers(Result<SubscribedParsersCollection> parsers) =>
        parsers.IsFailure ? parsers.Error : parsers.Value.PermanentlyEnableAll();

    private async Task<Result<SubscribedParsersCollection>> GetParsers(
        IEnumerable<Guid> identifiers,
        CancellationToken ct
    )
    {
        SubscribedParsersCollectionQuery query = new(Identifiers: identifiers);
        SubscribedParsersCollection parsers = await repository.Get(query, ct);
        return parsers.IsEmpty() ? Error.NotFound($"Парсеры не найдены.") : parsers;
    }
}
