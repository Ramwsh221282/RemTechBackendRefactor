using ParsersControl.Core.Contracts;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.ChangeLinkActivity;

[TransactionalHandler]
public sealed class ChangeLinkActivityCommandHandler(
    ISubscribedParsersRepository repository)
    : ICommandHandler<ChangeLinkActivityCommand, SubscribedParserLink>
{
    public async Task<Result<SubscribedParserLink>> Execute(
        ChangeLinkActivityCommand activityCommand, 
        CancellationToken ct = default)
    {
        Result<ISubscribedParser> parser = await GetRequiredParser(activityCommand.ParserId, ct);
        Result<SubscribedParserLink> link = ChangeLinkActivity(parser, activityCommand.LinkId, activityCommand.IsActive);
        return await SaveChanges(parser, link).Map(() => link.Value);
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser,
        Result<SubscribedParserLink> result)
    {
        if (result.IsFailure) return Result.Failure(result.Error);
        if (parser.IsFailure) return Result.Failure(parser.Error);
        await repository.Save(parser.Value);
        return Result.Success();
    }
    
    private Result<SubscribedParserLink> ChangeLinkActivity(
        Result<ISubscribedParser> parser, 
        Guid linkId, 
        bool isActive)
    {
        if (parser.IsFailure) return Result.Failure<SubscribedParserLink>(parser.Error);
        return parser.Value.ChangeLinkActivity(parser.Value.FindLink(linkId), isActive);
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(Guid parserId, CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: parserId, WithLock: true);
        return await repository.Get(query, ct: ct);
    }
}