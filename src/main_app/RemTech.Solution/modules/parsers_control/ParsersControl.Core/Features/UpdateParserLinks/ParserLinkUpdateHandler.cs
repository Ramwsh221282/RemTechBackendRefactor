using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.UpdateParserLinks;

[TransactionalHandler]
public sealed class ParserLinkUpdateHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<UpdateParserLinksCommand, IEnumerable<SubscribedParserLink>>
{
	public async Task<Result<IEnumerable<SubscribedParserLink>>> Execute(
		UpdateParserLinksCommand command,
		CancellationToken ct = default
	)
	{
		Result<SubscribedParser> parser = await GetParser(command.ParserId, ct);
		IEnumerable<ParserLinkUpdater> updaters = GetUpdaters(command);
		Result<IEnumerable<SubscribedParserLink>> result = UpdateLinks(parser, updaters);
		Result saving = await SaveChanges(parser, result, ct);
		return saving.IsFailure ? saving.Error : Result.Success(result.Value);
	}

	private static IEnumerable<ParserLinkUpdater> GetUpdaters(UpdateParserLinksCommand command) =>
		command.UpdateParameters.Select(p => ParserLinkUpdater.Create(p.LinkId, p.Activity, p.Name, p.Url).Value);

	private static Result<IEnumerable<SubscribedParserLink>> UpdateLinks(
		Result<SubscribedParser> parser,
		IEnumerable<ParserLinkUpdater> updaters
	) => parser.IsFailure ? parser.Error : parser.Value.UpdateLinks(updaters);

	private async Task<Result> SaveChanges(
		Result<SubscribedParser> parser,
		Result<IEnumerable<SubscribedParserLink>> links,
		CancellationToken ct
	)
	{
		if (parser.IsFailure)
			return Result.Failure(parser.Error);
		if (links.IsFailure)
			return Result.Failure(links.Error);
		await parser.Value.SaveChanges(repository, ct);
		return Result.Success();
	}

	private Task<Result<SubscribedParser>> GetParser(Guid id, CancellationToken ct)
	{
		SubscribedParserQuery query = new(Id: id);
		return SubscribedParser.FromRepository(repository, query, ct);
	}
}
