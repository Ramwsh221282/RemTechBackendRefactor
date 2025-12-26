using ParsersControl.Core.Contracts;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.SetLinkWorkTime;

[TransactionalHandler]
public sealed class SetLinkWorkingTimeHandler(
    ISubscribedParsersRepository parsers
) : ICommandHandler<SetLinkWorkingTimeCommand, SubscribedParserLink>
{
    public async Task<Result<SubscribedParserLink>> Execute(
        SetLinkWorkingTimeCommand command,
        CancellationToken ct = default)
    {
        Result<ISubscribedParser> parser = await GetRequiredParser(command.ParserId, ct);
        Result<SubscribedParserLink> link = SetLinkWorkTime(parser, command.LinkId, command.TotalElapsedSeconds);
        return await SaveChanges(parser, link).Map(() => link.Value);
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(Guid parserId, CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: parserId, WithLock: true);
        return await parsers.Get(query, ct);
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser,
        Result<SubscribedParserLink> link
    )
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (link.IsFailure) return Result.Failure(link.Error);
        await parsers.Save(parser.Value);
        return Result.Success();
    }

    private Result<SubscribedParserLink> SetLinkWorkTime(
        Result<ISubscribedParser> parser,
        Guid linkId,
        long totalElapsedSeconds)
    {
        if (parser.IsFailure) return Result.Failure<SubscribedParserLink>(parser.Error);
        Result<SubscribedParserLink> link = parser.Value.FindLink(linkId);
        if (link.IsFailure) return Result.Failure<SubscribedParserLink>(link.Error);
        return parser.Value.AddLinkWorkTime(link.Value, totalElapsedSeconds);
    }
}