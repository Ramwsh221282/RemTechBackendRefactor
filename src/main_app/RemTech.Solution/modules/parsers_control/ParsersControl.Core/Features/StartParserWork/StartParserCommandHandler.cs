using  ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.StartParserWork;

[TransactionalHandler]
public sealed class StartParserCommandHandler(ISubscribedParsersRepository repository) : ICommandHandler<StartParserCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(StartParserCommand command, CancellationToken ct = default)
    {
        Result<SubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<Unit> starting = CallParserWorkInvocation(parser);
        return await SaveChanges(parser, starting, ct).Map(() => parser.Value);
    }
    
    private async Task<Result<SubscribedParser>> GetRequiredParser(StartParserCommand command, CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        return await SubscribedParser.FromRepository(repository, query, ct);
    }

    private Result<Unit> CallParserWorkInvocation(Result<SubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.StartWork();
    }

    private async Task<Result> SaveChanges(Result<SubscribedParser> parser, Result<Unit> starting, CancellationToken ct)
    {
        if (starting.IsFailure) return Result.Failure(starting.Error);
        if (parser.IsFailure) return Result.Failure(parser.Error);
        await parser.Value.SaveChanges(repository, ct);
        return Result.Success();
    }
}