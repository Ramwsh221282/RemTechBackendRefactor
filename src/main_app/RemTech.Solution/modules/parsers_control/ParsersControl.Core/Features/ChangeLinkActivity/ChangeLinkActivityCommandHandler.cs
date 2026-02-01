using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.ChangeLinkActivity;

/// <summary>
/// Обработчик команды изменения активности ссылки парсера.
/// </summary>
/// <param name="repository">Репозиторий для работы с подписанными парсерами.</param>
[TransactionalHandler]
public sealed class ChangeLinkActivityCommandHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<ChangeLinkActivityCommand, SubscribedParserLink>
{
	/// <summary>
	/// Выполняет команду изменения активности ссылки парсера.
	/// </summary>
	/// <param name="activityCommand">Команда для выполнения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат выполнения команды с подписанной ссылкой парсера.</returns>
	public async Task<Result<SubscribedParserLink>> Execute(
		ChangeLinkActivityCommand activityCommand,
		CancellationToken ct = default
	)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(activityCommand.ParserId, ct);
		Result<SubscribedParserLink> link = ChangeLinkActivity(
			parser,
			activityCommand.LinkId,
			activityCommand.IsActive
		);
		Result saving = await SaveChanges(parser, link, ct);
		return saving.IsFailure ? saving.Error : link;
	}

	private static Result<SubscribedParserLink> ChangeLinkActivity(
		Result<SubscribedParser> parser,
		Guid linkId,
		bool isActive
	)
	{
		return parser.IsFailure
			? Result.Failure<SubscribedParserLink>(parser.Error)
			: parser.Value.ChangeLinkActivity(parser.Value.FindLink(linkId), isActive);
	}

	private async Task<Result> SaveChanges(
		Result<SubscribedParser> parser,
		Result<SubscribedParserLink> result,
		CancellationToken ct
	)
	{
		if (result.IsFailure)
		{
			return Result.Failure(result.Error);
		}

		if (parser.IsFailure)
		{
			return Result.Failure(parser.Error);
		}

		await parser.Value.SaveChanges(repository, ct);
		return Result.Success();
	}

	private Task<Result<SubscribedParser>> GetRequiredParser(Guid parserId, CancellationToken ct)
	{
		SubscribedParserQuery query = new(Id: parserId, WithLock: true);
		return SubscribedParser.FromRepository(repository, query, ct);
	}
}
