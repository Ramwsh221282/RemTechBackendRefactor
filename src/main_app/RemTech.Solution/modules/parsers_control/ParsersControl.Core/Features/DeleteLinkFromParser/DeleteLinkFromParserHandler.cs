using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.DeleteLinkFromParser;

/// <summary>
/// Обработчик команды удаления ссылки из парсера.
/// </summary>
/// <param name="repository">Репозиторий подписанных парсеров.</param>
[TransactionalHandler]
public sealed class DeleteLinkFromParserHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<DeleteLinkFromParserCommand, SubscribedParserLink>
{
	/// <summary>
	/// Выполняет команду удаления ссылки из парсера.
	/// </summary>
	/// <param name="command">Команда удаления ссылки из парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды с удалённой ссылкой.</returns>
	public async Task<Result<SubscribedParserLink>> Execute(
		DeleteLinkFromParserCommand command,
		CancellationToken ct = default
	)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command.ParserId, ct);
		Result<SubscribedParserLink> link = RemoveLink(parser, command.LinkId);
		Result saving = await SaveChanges(parser, link, ct);
		return saving.IsFailure ? saving.Error : link.Value;
	}

	private static Result<SubscribedParserLink> RemoveLink(Result<SubscribedParser> parser, Guid linkId)
	{
		if (parser.IsFailure)
			return Result.Failure<SubscribedParserLink>(parser.Error);
		Result<SubscribedParserLink> link = parser.Value.FindLink(linkId);
		return link.IsFailure ? Result.Failure<SubscribedParserLink>(link.Error) : parser.Value.RemoveLink(link.Value);
	}

	private async Task<Result> SaveChanges(
		Result<SubscribedParser> parser,
		Result<SubscribedParserLink> link,
		CancellationToken ct
	)
	{
		if (parser.IsFailure)
			return Result.Failure(parser.Error);
		if (link.IsFailure)
			return Result.Failure(link.Error);
		await repository.Save(parser.Value, ct);
		return Result.Success();
	}

	private Task<Result<SubscribedParser>> GetRequiredParser(Guid parserId, CancellationToken ct)
	{
		SubscribedParserQuery query = new(Id: parserId, WithLock: true);
		return SubscribedParser.FromRepository(repository, query, ct);
	}
}
