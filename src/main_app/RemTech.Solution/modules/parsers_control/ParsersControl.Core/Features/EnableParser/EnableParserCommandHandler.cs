using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.EnableParser;

/// <summary>
/// Обработчик команды <see cref="EnableParserCommand"/>.
/// </summary>
/// <param name="repository">Репозиторий для работы с подписанными парсерами.</param>
[TransactionalHandler]
public sealed class EnableParserCommandHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<EnableParserCommand, SubscribedParser>
{
	/// <summary>
	/// Выполнение команды включения парсера.
	/// </summary>
	/// <param name="command">Команда для выполнения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат выполнения команды с подписанным парсером.</returns>
	public async Task<Result<SubscribedParser>> Execute(EnableParserCommand command, CancellationToken ct = default)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command, ct);
		Result<Unit> enabled = Enable(parser);
		Result saving = await SaveChanges(parser, enabled, ct);
		return saving.IsFailure ? saving.Error : parser.Value;
	}

	private static Result<Unit> Enable(Result<SubscribedParser> parser) =>
		parser.IsFailure ? parser.Error : parser.Value.Enable();

	private async Task<Result> SaveChanges(Result<SubscribedParser> parser, Result<Unit> enabling, CancellationToken ct)
	{
		if (enabling.IsFailure)
			return Result.Failure(enabling.Error);
		if (parser.IsFailure)
			return Result.Failure(parser.Error);
		await repository.Save(parser.Value, ct);
		return Result.Success();
	}

	private Task<Result<SubscribedParser>> GetRequiredParser(EnableParserCommand command, CancellationToken ct)
	{
		SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
		return SubscribedParser.FromRepository(repository, query, ct);
	}
}
