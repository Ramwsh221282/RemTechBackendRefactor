using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.SetParsedAmount;

[TransactionalHandler]
public sealed class SetParserAmountCommandHandler(ISubscribedParsersRepository repository)
    : ICommandHandler<SetParsedAmountCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(SetParsedAmountCommand command, CancellationToken ct = default)
    {
        Result<SubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<Unit> result = SetRequiredAmount(command, parser);
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

    private Task<Result<SubscribedParser>> GetRequiredParser(
        SetParsedAmountCommand command,
        CancellationToken ct = default
    )
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        return SubscribedParser.FromRepository(repository, query, ct);
    }

    private static Result<Unit> SetRequiredAmount(SetParsedAmountCommand command, Result<SubscribedParser> parser) =>
        parser.IsFailure ? parser.Error : parser.Value.AddParserAmount(command.Amount);
}
