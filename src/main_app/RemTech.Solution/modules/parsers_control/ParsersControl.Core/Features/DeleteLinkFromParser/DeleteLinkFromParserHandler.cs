using ParsersControl.Core.Contracts;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.Features.DeleteLinkFromParser;

public sealed class DeleteLinkFromParserHandler(
    ISubscribedParsersRepository repository, 
    ITransactionSource source)
    : ICommandHandler<DeleteLinkFromParserCommand, SubscribedParserLink>
{
    public async Task<Result<SubscribedParserLink>> Execute(DeleteLinkFromParserCommand command, CancellationToken ct = default)
    {
        ITransactionScope scope = await source.BeginTransaction(ct);
        Result<ISubscribedParser> parser = await GetRequiredParser(command.ParserId, ct);
        Result<SubscribedParserLink> link = RemoveLink(parser, command.LinkId);
        return await SaveChanges(parser, link, scope, ct).Map(() => link.Value);
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser, 
        Result<SubscribedParserLink> link, 
        ITransactionScope txn, 
        CancellationToken ct)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (link.IsFailure) return Result.Failure(link.Error);
        await repository.Save(parser.Value);
        return await txn.Commit(ct);
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(
        Guid parserId,
        CancellationToken ct)
    {
        SubscribedParserQuery args = new(Id: parserId, WithLock: true);
        return await repository.Get(args, ct: ct);
    }

    private Result<SubscribedParserLink> RemoveLink(Result<ISubscribedParser> parser, Guid linkId)
    {
        if (parser.IsFailure) return Result.Failure<SubscribedParserLink>(parser.Error);
        Result<SubscribedParserLink> link = parser.Value.FindLink(linkId);
        if (link.IsFailure) return Result.Failure<SubscribedParserLink>(link.Error);
        return parser.Value.RemoveLink(link.Value);
    }
}