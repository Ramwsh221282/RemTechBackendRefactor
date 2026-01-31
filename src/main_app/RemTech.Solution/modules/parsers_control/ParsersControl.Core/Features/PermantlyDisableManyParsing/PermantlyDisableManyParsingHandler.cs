using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.PermantlyDisableManyParsing;

/// <summary>
/// Обработчик команды <see cref="PermantlyDisableManyParsingCommand"/>.
/// </summary>
/// <param name="repository">Репозиторий для работы с коллекцией подписанных парсеров.</param>
[TransactionalHandler]
public sealed class PermantlyDisableManyParsingHandler(ISubscribedParsersCollectionRepository repository)
	: ICommandHandler<PermantlyDisableManyParsingCommand, IEnumerable<SubscribedParser>>
{
	/// <summary>
	/// Выполнение команды отключения списка подписанных парсеров.
	/// </summary>
	/// <param name="command">Команда для выполнения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат выполнения команды с коллекцией подписанных парсеров.</returns>
	public async Task<Result<IEnumerable<SubscribedParser>>> Execute(
		PermantlyDisableManyParsingCommand command,
		CancellationToken ct = default
	)
	{
		Result<SubscribedParsersCollection> parsers = await GetParsers(command.Identifiers, ct);
		Result<Unit> result = PermantlyDisableParsers(parsers);
		Result<Unit> saving = await SaveChanges(result, parsers, ct);
		return saving.IsFailure ? saving.Error : Result.Success(parsers.Value.Read());
	}

	private static Result<Unit> PermantlyDisableParsers(Result<SubscribedParsersCollection> parsers) =>
		parsers.IsFailure ? (Result<Unit>)parsers.Error : parsers.Value.PermanentlyDisableAll();

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
		SubscribedParsersCollection parsers = await repository.Read(query, ct);
		return parsers.IsEmpty() ? Error.NotFound("Парсеры не найдены.") : parsers;
	}
}
