using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.ChangeWaitDays;

[TransactionalHandler]
public sealed class ChangeWaitDaysHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<ChangeWaitDaysCommand, SubscribedParser>
{
	public async Task<Result<SubscribedParser>> Execute(ChangeWaitDaysCommand command, CancellationToken ct = default)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command.Id, ct);
		Result<Unit> result = SetRequiredWaitDays(command.WaitDays, parser);
		Result<Unit> saving = await SaveChanges(parser, result, ct);
		return saving.IsSuccess ? parser.Value : saving.Error;
	}

	private static Result<Unit> SetRequiredWaitDays(int days, Result<SubscribedParser> parser)
	{
		return parser.IsFailure ? (Result<Unit>)parser.Error : parser.Value.ChangeScheduleWaitDays(days);
	}

	private async Task<Result<Unit>> SaveChanges(
		Result<SubscribedParser> parser,
		Result<Unit> result,
		CancellationToken ct
	)
	{
		if (parser.IsFailure)
			return parser.Error;
		if (result.IsFailure)
			return result.Error;
		await parser.Value.SaveChanges(repository, ct);
		return Unit.Value;
	}

	private Task<Result<SubscribedParser>> GetRequiredParser(Guid id, CancellationToken ct)
	{
		SubscribedParserQuery query = new SubscribedParserQuery().WithId(id).RequireLock();
		return SubscribedParser.FromRepository(repository, query, ct);
	}
}
