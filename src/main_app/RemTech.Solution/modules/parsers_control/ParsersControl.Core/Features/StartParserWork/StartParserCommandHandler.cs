using  ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.StartParserWork;

[TransactionalHandler]
public sealed class StartParserCommandHandler(
    ISubscribedParsersRepository repository,
    IOnParserStartedListener listener
) : ICommandHandler<StartParserCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(StartParserCommand command, CancellationToken ct = default)
    {
        Result<ISubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<SubscribedParser> starting = CallParserWorkInvocation(parser);
        Result saving = await SaveChanges(parser, starting);
        if (saving.IsFailure) return saving.Error;
        await NotifyListener(saving, starting.Value);
        return starting;
    }

    private async Task NotifyListener(Result saveResult, SubscribedParser parser)
    {
        if (saveResult.IsFailure) return;
        await listener.Handle(parser);
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(StartParserCommand command, CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        Result<ISubscribedParser> parser = await repository.Get(query, ct: ct);
        return parser;
    }

    private Result<SubscribedParser> CallParserWorkInvocation(Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.StartWork();
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser, 
        Result<SubscribedParser> starting)
    {
        if (starting.IsFailure) return Result.Failure(starting.Error);
        if (parser.IsFailure) return Result.Failure(parser.Error);
        await repository.Save(parser.Value);
        return Result.Success();
    }
}