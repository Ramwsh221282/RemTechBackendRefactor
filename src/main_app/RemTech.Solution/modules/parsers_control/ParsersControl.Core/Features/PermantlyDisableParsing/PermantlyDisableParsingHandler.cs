using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.PermantlyDisableParsing;

[TransactionalHandler]
public sealed class PermantlyDisableParsingHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<PermantlyDisableParsingCommand, SubscribedParser>
{
	public async Task<Result<SubscribedParser>> Execute(
		PermantlyDisableParsingCommand command,
		CancellationToken ct = default
	)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command.Id, ct);
		Result<Unit> result = InvokePermantlyDisable(parser);
		Result saving = await SaveChanges(parser, result, ct);
		return saving.IsFailure ? saving.Error : parser.Value;
	}

	private async Task<Result> SaveChanges(Result<SubscribedParser> parser, Result<Unit> result, CancellationToken ct)
	{
		if (parser.IsFailure)
			return Result.Failure(parser.Error);
		if (result.IsFailure)
			return Result.Failure(result.Error);

		await parser.Value.SaveChanges(repository, ct);
		return Result.Success();
	}

	private static Result<Unit> InvokePermantlyDisable(Result<SubscribedParser> parser)
	{
		if (parser.IsFailure)
			return parser.Error;

		parser.Value.PermanentlyDisable();
		return Unit.Value;
	}

	private async Task<Result<SubscribedParser>> GetRequiredParser(Guid id, CancellationToken ct)
	{
		SubscribedParserQuery query = new(Id: id, WithLock: true);
		return await SubscribedParser.FromRepository(repository, query, ct);
	}
}
