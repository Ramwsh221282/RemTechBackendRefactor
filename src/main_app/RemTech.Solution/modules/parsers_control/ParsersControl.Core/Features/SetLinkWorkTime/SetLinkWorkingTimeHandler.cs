using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.SetLinkWorkTime;

[TransactionalHandler]
public sealed class SetLinkWorkingTimeHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<SetLinkWorkingTimeCommand, SubscribedParserLink>
{
	public async Task<Result<SubscribedParserLink>> Execute(
		SetLinkWorkingTimeCommand command,
		CancellationToken ct = default
	)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command.ParserId, ct);
		Result<SubscribedParserLink> link = SetLinkWorkTime(parser, command.LinkId, command.TotalElapsedSeconds);
		Result saving = await SaveChanges(parser, link, ct);
		return saving.IsFailure ? saving.Error : link.Value;
	}

	private static Result<SubscribedParserLink> SetLinkWorkTime(
		Result<SubscribedParser> parser,
		Guid linkId,
		long totalElapsedSeconds
	)
	{
		if (parser.IsFailure)
			return Result.Failure<SubscribedParserLink>(parser.Error);
		Result<SubscribedParserLink> link = parser.Value.FindLink(linkId);
		return link.IsFailure
			? Result.Failure<SubscribedParserLink>(link.Error)
			: parser.Value.AddLinkWorkTime(link.Value, totalElapsedSeconds);
	}

	private Task<Result<SubscribedParser>> GetRequiredParser(Guid parserId, CancellationToken ct)
	{
		SubscribedParserQuery query = new(Id: parserId, WithLock: true);
		return SubscribedParser.FromRepository(repository, query, ct);
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
		await parser.Value.SaveChanges(repository, ct);
		return Result.Success();
	}
}
