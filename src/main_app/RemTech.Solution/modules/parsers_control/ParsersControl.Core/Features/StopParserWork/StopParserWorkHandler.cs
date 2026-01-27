using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.StopParserWork;

/// <summary>
/// Обработчик команды остановки работы парсера.
/// </summary>
/// <param name="repository">Репозиторий подписанных парсеров.</param>
[TransactionalHandler]
public sealed class StopParserWorkHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<StopParserWorkCommand, SubscribedParser>
{
	/// <summary>
	/// Выполняет команду остановки работы парсера.
	/// </summary>
	/// <param name="command">Команда остановки работы парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды с обновленным парсером.</returns>
	public async Task<Result<SubscribedParser>> Execute(StopParserWorkCommand command, CancellationToken ct = default)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command, ct);
		Result<Unit> finished = FinishWork(parser);
		Result saving = await SaveChanges(parser, finished, ct);
		return saving.IsFailure ? (Result<SubscribedParser>)saving.Error : (Result<SubscribedParser>)parser.Value;
	}

	private static Result<Unit> FinishWork(Result<SubscribedParser> parser) =>
		parser.IsFailure ? parser.Error : parser.Value.FinishWork();

	private async Task<Result> SaveChanges(Result<SubscribedParser> parser, Result<Unit> finished, CancellationToken ct)
	{
		if (finished.IsFailure)
			return Result.Failure(finished.Error);
		if (parser.IsFailure)
			return Result.Failure(parser.Error);
		await parser.Value.SaveChanges(repository, ct);
		return Result.Success();
	}

	private Task<Result<SubscribedParser>> GetRequiredParser(
		StopParserWorkCommand command,
		CancellationToken ct = default
	)
	{
		SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
		return SubscribedParser.FromRepository(repository, query, ct);
	}
}
