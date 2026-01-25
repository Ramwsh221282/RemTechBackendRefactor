using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.DisableParser;

[TransactionalHandler]
public sealed class DisableParserHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<DisableParserCommand, SubscribedParser>
{
	public async Task<Result<SubscribedParser>> Execute(DisableParserCommand command, CancellationToken ct = default)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command.Id, ct);
		Result<Unit> result = Disable(parser);
		Result<Unit> saving = await SaveChanges(parser, result, ct);
		return saving.IsSuccess ? parser.Value : saving.Error;
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
		return Result.Success(Unit.Value);
	}

	private static Result<Unit> Disable(Result<SubscribedParser> parser)
	{
		if (parser.IsFailure)
			return parser.Error;
		if (parser.Value.State.IsWorking())
			return Error.Conflict("Парсер в рабочем состоянии. Отключить можно только перманентно.");
		if (parser.Value.State.IsDisabled())
			return Error.Conflict("Парсер уже отключен.");
		parser.Value.Disable();
		return Unit.Value;
	}

	private Task<Result<SubscribedParser>> GetRequiredParser(Guid id, CancellationToken ct)
	{
		SubscribedParserQuery query = new SubscribedParserQuery().WithId(id).RequireLock();
		return SubscribedParser.FromRepository(repository, query, ct);
	}
}
