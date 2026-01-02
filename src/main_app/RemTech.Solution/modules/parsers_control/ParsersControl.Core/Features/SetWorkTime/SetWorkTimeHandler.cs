using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.SetWorkTime;

[TransactionalHandler]
public sealed class SetWorkTimeHandler(ISubscribedParsersRepository repository) : ICommandHandler<SetWorkTimeCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(SetWorkTimeCommand command, CancellationToken ct = default)
    {
        Result<SubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<Unit> result = SetRequiredTime(command, parser);
        return await SaveChanges(parser, result, ct).Map(() => parser.Value);
    }

    private async Task<Result> SaveChanges(Result<SubscribedParser> parser, Result<Unit> result, CancellationToken ct)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (result.IsFailure) return Result.Failure(result.Error);
        await parser.Value.SaveChanges(repository, ct);
        return Result.Success();
    }
    
    private async Task<Result<SubscribedParser>> GetRequiredParser(SetWorkTimeCommand command, CancellationToken ct = default)
    {
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        return await SubscribedParser.FromRepository(repository, query, ct);
    }

    private Result<Unit> SetRequiredTime(SetWorkTimeCommand command, Result<SubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.AddWorkTime(command.TotalElapsedSeconds);
    }
}