using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.PermantlyStartManyParsing;

/// <summary>
/// Обработчик команды постоянного запуска множества парсеров.
/// </summary>
/// <param name="repository">Репозиторий коллекции подписанных парсеров.</param>
[TransactionalHandler]
public sealed class PermantlyStartManyParsingHandler(ISubscribedParsersCollectionRepository repository)
	: ICommandHandler<PermantlyStartManyParsingCommand, IEnumerable<SubscribedParser>>
{
	/// <summary>
	///   Выполняет команду постоянного запуска множества парсеров.
	/// </summary>
	/// <param name="command">Команда постоянного запуска множества парсеров.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды с запущенными парсерами.</returns>
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

	private static Result<Unit> PermanentlyStartParsers(Result<SubscribedParsersCollection> parsers) =>
		parsers.IsFailure ? parsers.Error : parsers.Value.PermanentlyEnableAll();

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
		return await repository.SaveChanges(parsers.Value, ct);
	}

	private async Task<Result<SubscribedParsersCollection>> GetParsers(
		IEnumerable<Guid> identifiers,
		CancellationToken ct
	)
	{
		SubscribedParsersCollectionQuery query = new(Identifiers: identifiers);
		SubscribedParsersCollection parsers = await repository.Get(query, ct);
		return parsers.IsEmpty() ? Error.NotFound("Парсеры не найдены.") : parsers;
	}
}
