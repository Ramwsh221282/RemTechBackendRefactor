using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.SetWorkTime;

[TransactionalHandler]
public sealed class SetWorkTimeHandler(ISubscribedParsersRepository repository)
	: ICommandHandler<SetWorkTimeCommand, SubscribedParser>
{
	public async Task<Result<SubscribedParser>> Execute(SetWorkTimeCommand command, CancellationToken ct = default)
	{
		Result<SubscribedParser> parser = await GetRequiredParser(command, ct);
		Result<Unit> result = SetRequiredTime(command, parser);
		Result saving = await SaveChanges(parser, result, ct);
		return saving.IsFailure ? (Result<SubscribedParser>)saving.Error : (Result<SubscribedParser>)parser.Value;
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

	private async Task<Result<SubscribedParser>> GetRequiredParser(
		SetWorkTimeCommand command,
		CancellationToken ct = default
	)
	{
		SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
		return await SubscribedParser.FromRepository(repository, query, ct);
	}

	private static Result<Unit> SetRequiredTime(SetWorkTimeCommand command, Result<SubscribedParser> parser) =>
		parser.IsFailure ? parser.Error : parser.Value.AddWorkTime(command.TotalElapsedSeconds);
}
