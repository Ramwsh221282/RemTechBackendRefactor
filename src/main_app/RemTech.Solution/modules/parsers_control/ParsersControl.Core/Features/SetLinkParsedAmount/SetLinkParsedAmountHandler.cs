using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.SetLinkParsedAmount;

[TransactionalHandler]
public sealed class SetLinkParsedAmountHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<SetLinkParsedAmountCommand, SubscribedParserLink>
{
	public async Task<Result<SubscribedParserLink>> Execute(
		SetLinkParsedAmountCommand command,
		CancellationToken ct = default
	)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command.ParserId, ct);
		Result<SubscribedParserLink> link = SetLinkParsedAmount(parser, command.LinkId, command.Amount);
		Result result = await SaveChanges(parser, link, ct);
		return result.IsFailure ? result.Error : link.Value;
	}

	private static Result<SubscribedParserLink> SetLinkParsedAmount(
		Result<SubscribedParser> parser,
		Guid linkId,
		int amount
	)
	{
		if (parser.IsFailure)
			return parser.Error;

		Result<SubscribedParserLink> link = parser.Value.FindLink(linkId);
		return link.IsFailure ? link.Error : parser.Value.AddLinkParsedAmount(link.Value, amount);
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

	private Task<Result<SubscribedParser>> GetRequiredParser(Guid parserId, CancellationToken ct)
	{
		SubscribedParserQuery query = new(Id: parserId, WithLock: true);
		return SubscribedParser.FromRepository(repository, query, ct);
	}
}
