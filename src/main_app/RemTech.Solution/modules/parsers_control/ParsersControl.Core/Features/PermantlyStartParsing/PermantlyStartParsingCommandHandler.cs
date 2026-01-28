using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.PermantlyStartParsing;

/// <summary>
/// Обработчик команды постоянного запуска парсера.
/// </summary>
/// <param name="repository">Репозиторий подписанных парсеров.</param>
[TransactionalHandler]
public sealed class PermantlyStartParsingCommandHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<PermantlyStartParsingCommand, SubscribedParser>
{
	/// <summary>
	///  Выполняет команду постоянного запуска парсера.
	/// </summary>
	/// <param name="command">Команда постоянного запуска парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды с запущенным парсером.</returns>
	public async Task<Result<SubscribedParser>> Execute(
		PermantlyStartParsingCommand command,
		CancellationToken ct = default
	)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command.Id, ct);
		Result<Unit> result = StartParser(parser);
		Result saving = await SaveChanges(parser, result, ct);
		return saving.IsFailure ? saving.Error : parser.Value;
	}

	private static Result<Unit> StartParser(Result<SubscribedParser> parser) =>
		parser.IsFailure ? parser.Error : parser.Value.PermantlyEnable();

	private async Task<Result> SaveChanges(Result<SubscribedParser> parser, Result<Unit> result, CancellationToken ct)
	{
		if (parser.IsFailure)
			return Result.Failure(parser.Error);
		if (result.IsFailure)
			return Result.Failure(result.Error);

		await parser.Value.SaveChanges(repository, ct);
		return Result.Success();
	}

	private Task<Result<SubscribedParser>> GetRequiredParser(Guid parserId, CancellationToken ct)
	{
		SubscribedParserQuery query = new(Id: parserId, WithLock: true);
		return SubscribedParser.FromRepository(repository, query, ct);
	}
}
