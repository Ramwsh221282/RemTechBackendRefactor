using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.FinishParser;

/// <summary>
/// Обработчик команды <see cref="FinishParserCommand"/>.
/// </summary>
/// <param name="repository">Репозиторий для работы с подписанными парсерами.</param>
[TransactionalHandler]
public sealed class FinishParserHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<FinishParserCommand, SubscribedParser>
{
	/// <summary>
	/// Выполнение команды завершения работы парсера.
	/// </summary>
	/// <param name="command">Команда для выполнения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат выполнения команды с подписанным парсером.</returns>
	public async Task<Result<SubscribedParser>> Execute(FinishParserCommand command, CancellationToken ct = default)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command.Id, ct);
		Result<Unit> result = FinishParser(parser, command.TotalElapsedSeconds);
		Result saving = await SaveChanges(parser, result, ct);
		return saving.IsFailure ? saving.Error : parser.Value;
	}

	private static Result<Unit> FinishParser(Result<SubscribedParser> parser, long totalElapsedSeconds) =>
		parser.IsFailure ? parser.Error : parser.Value.FinishWork(totalElapsedSeconds);

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
