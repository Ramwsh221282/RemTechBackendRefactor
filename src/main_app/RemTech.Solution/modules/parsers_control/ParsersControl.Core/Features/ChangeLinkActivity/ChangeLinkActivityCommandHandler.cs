using ParsersControl.Core.Contracts;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.Features.ChangeLinkActivity;

public sealed class ChangeLinkActivityCommandHandler(
    ISubscribedParsersRepository repository,
    ITransactionSource transactionSource)
    : ICommandHandler<ChangeLinkActivityCommand, SubscribedParserLink>
{
    public async Task<Result<SubscribedParserLink>> Execute(
        ChangeLinkActivityCommand activityCommand, 
        CancellationToken ct = default)
    {
        ITransactionScope scope = await transactionSource.BeginTransaction(ct);
        Result<ISubscribedParser> parser = await GetRequiredParser(activityCommand.ParserId, ct);
        Result<SubscribedParserLink> link = ChangeLinkActivity(parser, activityCommand.LinkId, activityCommand.IsActive);
        return await SaveChanges(parser, link, scope, ct).Map(() => link.Value);
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser,
        Result<SubscribedParserLink> result,
        ITransactionScope scope,
        CancellationToken ct)
    {
        if (result.IsFailure) return Result.Failure(result.Error);
        await repository.Save(parser.Value);
        Result commit = await scope.Commit(ct: ct);
        return commit;
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