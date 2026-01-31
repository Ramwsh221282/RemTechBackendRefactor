using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.StartParserWork;

/// <summary>
/// Обработчик команды запуска парсера.
/// </summary>
/// <param name="repository">Репозиторий подписанных парсеров.</param>
[TransactionalHandler]
public sealed class StartParserCommandHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<StartParserCommand, SubscribedParser>
{
	/// <summary>
	/// Выполняет команду запуска парсера.
	/// </summary>
	/// <param name="command">Команда запуска парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды с обновленным парсером.</returns>
	public async Task<Result<SubscribedParser>> Execute(StartParserCommand command, CancellationToken ct = default)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command, ct);
		Result<Unit> starting = CallParserWorkInvocation(parser);
		Result result = await SaveChanges(parser, starting, ct);
		return result.IsFailure ? result.Error : parser.Value;
	}

	private static Result<Unit> CallParserWorkInvocation(Result<SubscribedParser> parser)
	{
		return parser.IsFailure ? parser.Error : parser.Value.StartWaiting();
	}

	private Task<Result<SubscribedParser>> GetRequiredParser(StartParserCommand command, CancellationToken ct)
	{
		SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
		return SubscribedParser.FromRepository(repository, query, ct);
	}

	private async Task<Result> SaveChanges(Result<SubscribedParser> parser, Result<Unit> starting, CancellationToken ct)
	{
		if (starting.IsFailure)
		{
			return Result.Failure(starting.Error);
		}

		if (parser.IsFailure)
		{
			return Result.Failure(parser.Error);
		}

		await parser.Value.SaveChanges(repository, ct);
		return Result.Success();
	}
}
