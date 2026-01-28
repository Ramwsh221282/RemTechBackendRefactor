using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.EditLinkUrlInfo;

/// <summary>
/// Обработчик команды редактирования информации о ссылке и URL парсера.
/// </summary>
/// <param name="repository">Репозиторий подписанных парсеров.</param>
[TransactionalHandler]
public sealed class EditLinkUrlInfoHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<EditLinkUrlInfoCommand, SubscribedParserLink>
{
	/// <summary>
	/// Выполняет команду редактирования информации о ссылке и URL парсера.
	/// </summary>
	/// <param name="command">Команда редактирования информации о ссылке и URL парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды с отредактированной ссылкой парсера.</returns>
	public async Task<Result<SubscribedParserLink>> Execute(
		EditLinkUrlInfoCommand command,
		CancellationToken ct = default
	)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command.ParserId, ct);
		Result<SubscribedParserLink> link = EditLink(parser, command.LinkId, command.NewName, command.NewUrl);
		Result saving = await SaveChanges(parser, link, ct);
		return saving.IsFailure ? saving.Error : link.Value;
	}

	private static Result<SubscribedParserLink> EditLink(
		Result<SubscribedParser> parser,
		Guid linkId,
		string? newName,
		string? newUrl
	)
	{
		if (parser.IsFailure)
			return Result.Failure<SubscribedParserLink>(parser.Error);
		Result<SubscribedParserLink> link = parser.Value.FindLink(linkId);
		if (link.IsFailure)
			return Result.Failure<SubscribedParserLink>(link.Error);
		Result<SubscribedParserLink> editResult = parser.Value.EditLink(link.Value, newName, newUrl);
		return editResult.IsFailure ? Result.Failure<SubscribedParserLink>(editResult.Error) : editResult;
	}

	private Task<Result<SubscribedParser>> GetRequiredParser(Guid parserId, CancellationToken ct)
	{
		SubscribedParserQuery query = new(Id: parserId, WithLock: true);
		return SubscribedParser.FromRepository(repository, query, ct);
	}

	private async Task<Result> SaveChanges(
		Result<SubscribedParser> parser,
		Result<SubscribedParserLink> editResult,
		CancellationToken ct
	)
	{
		if (parser.IsFailure)
			return Result.Failure(parser.Error);
		if (editResult.IsFailure)
			return Result.Failure(editResult.Error);
		await parser.Value.SaveChanges(repository, ct);
		return Result.Success();
	}
}
