using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.PermantlyStartParsing;

[TransactionalHandler]
public sealed class PermantlyStartParsingCommandHandler(
    ISubscribedParsersRepository repository
)
    : ICommandHandler<PermantlyStartParsingCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(PermantlyStartParsingCommand command, CancellationToken ct = default)
    {
        Result<ISubscribedParser> parser = await GetRequiredParser(command.Id, ct);
        Result<SubscribedParser> result = StartParser(parser);
        Result saveResult = await SaveChanges(parser, result);
        return saveResult.IsFailure ? saveResult.Error : result;
    }

    private async Task<Result> SaveChanges(Result<ISubscribedParser> parser, Result<SubscribedParser> result)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (result.IsFailure) return Result.Failure(result.Error);
        await repository.Save(parser.Value);
        return Result.Success();
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(Guid parserId, CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: parserId, WithLock: true);
        Result<ISubscribedParser> parser = await repository.Get(query, ct);
        return parser;
    }

    private Result<SubscribedParser> StartParser(Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.Enable();
    }
}