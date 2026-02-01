using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.ChangeSchedule;

/// <summary>
/// Обработчик команды изменения расписания парсера.
/// </summary>
/// <param name="repository">Репозиторий для работы с подписанными парсерами.</param>
[TransactionalHandler]
public sealed class ChangeScheduleCommandHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<ChangeScheduleCommand, SubscribedParser>
{
	/// <summary>
	/// Выполняет команду изменения расписания парсера.
	/// </summary>
	/// <param name="command">Команда для выполнения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат выполнения команды с подписанным парсером.</returns>
	public async Task<Result<SubscribedParser>> Execute(ChangeScheduleCommand command, CancellationToken ct = default)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command, ct);
		Result<Unit> result = SetRequiredSchedule(command, parser);
		Result saving = await SaveChanges(parser, result, ct);
		return saving.IsFailure ? saving.Error : parser.Value;
	}

	private static Result<Unit> SetRequiredSchedule(ChangeScheduleCommand command, Result<SubscribedParser> parser)
	{
		return parser.IsFailure ? (Result<Unit>)parser.Error : parser.Value.ChangeScheduleWaitDays(command.WaitDays);
	}

	private async Task<Result> SaveChanges(Result<SubscribedParser> parser, Result<Unit> result, CancellationToken ct)
	{
		if (parser.IsFailure)
		{
			return Result.Failure(parser.Error);
		}

		if (result.IsFailure)
		{
			return Result.Failure(result.Error);
		}

		await parser.Value.SaveChanges(repository, ct);
		return Result.Success();
	}

	private Task<Result<SubscribedParser>> GetRequiredParser(ChangeScheduleCommand command, CancellationToken ct)
	{
		SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
		return SubscribedParser.FromRepository(repository, query, ct);
	}
}
