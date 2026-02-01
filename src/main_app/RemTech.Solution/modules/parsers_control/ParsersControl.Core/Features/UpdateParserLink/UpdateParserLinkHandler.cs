using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Features.UpdateParserLinks;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.UpdateParserLink;

/// <summary>
/// Обработчик команды обновления ссылки парсера.
/// </summary>
/// <param name="repository">Репозиторий подписанных парсеров.</param>
[TransactionalHandler]
public sealed class UpdateParserLinkHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<UpdateParserLinkCommand, SubscribedParserLink>
{
	/// <summary>
	/// Выполняет команду обновления ссылки парсера.
	/// </summary>
	/// <param name="command">Команда обновления ссылки парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды с обновленной ссылкой парсера.</returns>
	public async Task<Result<SubscribedParserLink>> Execute(
		UpdateParserLinkCommand command,
		CancellationToken ct = default
	)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command.ParserId, ct);
		Result<SubscribedParserLink> link = FindLink(parser, command.LinkId);
		Result<ParserLinkUpdater> updater = CreateUpdater(parser, link, command.Name, command.Url);
		Result<SubscribedParserLink> updated = UpdateLink(parser, link, updater);
		Result<Unit> saved = await SaveChanges(parser, link, updater, updated, ct);
		return saved.IsSuccess ? parser.Value.FindLink(command.LinkId) : saved.Error;
	}

	private static Result<SubscribedParserLink> UpdateLink(
		Result<SubscribedParser> parser,
		Result<SubscribedParserLink> link,
		Result<ParserLinkUpdater> updater
	)
	{
		if (parser.IsFailure)
		{
			return parser.Error;
		}
		if (link.IsFailure)
		{
			return link.Error;
		}
		if (updater.IsFailure)
		{
			return updater.Error;
		}
		Result<IEnumerable<SubscribedParserLink>> updated = parser.Value.UpdateLinks([updater.Value]);
		return updated.IsFailure ? updated.Error : updated.Value.First();
	}

	private static Result<ParserLinkUpdater> CreateUpdater(
		Result<SubscribedParser> parser,
		Result<SubscribedParserLink> link,
		string? name,
		string? url
	)
	{
		if (parser.IsFailure)
		{
			return parser.Error;
		}

		return link.IsFailure
			? (Result<ParserLinkUpdater>)link.Error
			: ParserLinkUpdater.Create(link.Value.Id.Value, activity: null, name: name, url: url);
	}

	private static Result<SubscribedParserLink> FindLink(Result<SubscribedParser> parser, Guid linkId)
	{
		return parser.IsFailure ? parser.Error : parser.Value.FindLink(linkId);
	}

	private Task<Result<SubscribedParser>> GetRequiredParser(Guid parserId, CancellationToken ct)
	{
		SubscribedParserQuery query = new SubscribedParserQuery().WithId(parserId).RequireLock();
		return SubscribedParser.FromRepository(repository, query, ct);
	}

	private async Task<Result<Unit>> SaveChanges(
		Result<SubscribedParser> parser,
		Result<SubscribedParserLink> link,
		Result<ParserLinkUpdater> updater,
		Result<SubscribedParserLink> updated,
		CancellationToken ct
	)
	{
		if (parser.IsFailure)
		{
			return Result.Failure<Unit>(parser.Error);
		}
		if (link.IsFailure)
		{
			return Result.Failure<Unit>(link.Error);
		}
		if (updater.IsFailure)
		{
			return Result.Failure<Unit>(updater.Error);
		}
		if (updated.IsFailure)
		{
			return Result.Failure<Unit>(updated.Error);
		}

		await parser.Value.SaveChanges(repository, ct);
		return Unit.Value;
	}
}
