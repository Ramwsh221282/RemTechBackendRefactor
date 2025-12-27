using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.PermantlyDisableParsing;

[TransactionalHandler]
public sealed class PermantlyDisableParsingHandler(
    ISubscribedParsersRepository repository
) 
    : ICommandHandler<PermantlyDisableParsingCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(PermantlyDisableParsingCommand command, CancellationToken ct = default)
    {
        Result<ISubscribedParser> parser = await GetRequiredParser(command.Id);
        Result<SubscribedParser> result = InvokePermantlyDisable(parser);
        Result saving = await SaveChanges(parser, result);
        if (saving.IsFailure) return saving.Error;
        return result;
    }

    private async Task<Result> SaveChanges(Result<ISubscribedParser> parser, Result<SubscribedParser> result)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (result.IsFailure) return Result.Failure(result.Error);
        await repository.Save(parser.Value);
        return Result.Success();
    }
    
    private Result<SubscribedParser> InvokePermantlyDisable(Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return Result.Success(parser.Value.PermantlyDisable());
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(Guid id)
    {
        SubscribedParserQuery query = new(Id: id, WithLock: true);
        return await repository.Get(query);
    }
}