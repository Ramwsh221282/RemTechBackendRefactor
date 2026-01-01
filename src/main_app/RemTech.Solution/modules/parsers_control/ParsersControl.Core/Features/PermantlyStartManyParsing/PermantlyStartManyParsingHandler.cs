using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.PermantlyStartManyParsing;

[TransactionalHandler]
public sealed class PermantlyStartManyParsingHandler(ISubscribedParsersCollectionRepository repository) 
    : ICommandHandler<PermantlyStartManyParsingCommand, IEnumerable<SubscribedParser>>
{
    public async Task<Result<IEnumerable<SubscribedParser>>> Execute(
        PermantlyStartManyParsingCommand command, 
        CancellationToken ct = default)
    {
        Result<SubscribedParsersCollection> parsers = await GetParsers(command.Identifiers, ct);
        Result<Unit> result = PermanentlyStartParsers(parsers);
        Result<Unit> saving = await SaveChanges(result, parsers, ct);
        return saving.Map(() => Result.Success(parsers.Value.Read()));
    }

    private async Task<Result<Unit>> SaveChanges(Result<Unit> enabling, Result<SubscribedParsersCollection> parsers, CancellationToken ct)
    {
        if (parsers.IsFailure) return parsers.Error;
        if (enabling.IsFailure) return enabling.Error;
        Result<Unit> saving = await repository.SaveChanges(parsers.Value, ct);
        return saving;
    }
    
    private Result<Unit> PermanentlyStartParsers(Result<SubscribedParsersCollection> parsers)
    {
        if (parsers.IsFailure) return parsers.Error;
        return parsers.Value.PermanentlyEnableAll();
    }
    
    private async Task<Result<SubscribedParsersCollection>> GetParsers(IEnumerable<Guid> identifiers, CancellationToken ct)
    {
        SubscribedParsersCollectionQuery query = new(Identifiers: identifiers);
        SubscribedParsersCollection parsers = await repository.Get(query, ct);
        if (parsers.IsEmpty()) return Error.NotFound($"Парсеры не найдены.");
        return parsers;
    }
}