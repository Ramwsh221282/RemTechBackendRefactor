using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.SetParsedAmount;
    
[TransactionalHandler]
public sealed class SetParserAmountCommandHandler(
    ISubscribedParsersRepository repository
) : ICommandHandler<SetParsedAmountCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(
        SetParsedAmountCommand command, 
        CancellationToken ct = default)
    {
        Result<ISubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<SubscribedParser> result = SetRequiredAmount(command, parser);
        return await SaveChanges(parser, result).Map(() => result.Value);
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser, 
        Result<SubscribedParser> result)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (result.IsFailure) return Result.Failure(result.Error);
        await repository.Save(parser.Value);
        return Result.Success();
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(
        SetParsedAmountCommand command,
        CancellationToken ct = default)
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        Result<ISubscribedParser> parser = await repository.Get(query, ct: ct);
        return parser;
    }
    
    private Result<SubscribedParser> SetRequiredAmount(
        SetParsedAmountCommand command, 
        Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.AddParserAmount(command.Amount);
    }
}