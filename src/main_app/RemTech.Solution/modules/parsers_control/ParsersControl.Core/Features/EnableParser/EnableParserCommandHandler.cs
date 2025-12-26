using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.EnableParser;

[TransactionalHandler]
public sealed class EnableParserCommandHandler(
    ISubscribedParsersRepository repository
) : ICommandHandler<EnableParserCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(
        EnableParserCommand command, 
        CancellationToken ct = default)
    {
        Result<ISubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<SubscribedParser> enabled = Enable(parser);
        return await SaveChanges(parser).Map(() => enabled.Value);
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        await repository.Save(parser.Value);
        return Result.Success();
    }
    
    private Result<SubscribedParser> Enable(Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.Enable();
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(
        EnableParserCommand command,
        CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        Result<ISubscribedParser> parser = await repository.Get(query, ct: ct);
        return parser;
    }
}