using ParsersControl.Core.Contracts;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.Features.AddParserLink;

public sealed class AddParserLinkHandler(
    ISubscribedParsersRepository repository,
    ITransactionSource transactionSource) : ICommandHandler<AddParserLinkCommand, SubscribedParserLink>
{
    public async Task<Result<SubscribedParserLink>> Execute(
        AddParserLinkCommand command, 
        CancellationToken ct = default)
    {
        ITransactionScope scope = await transactionSource.BeginTransaction(ct);
        Result<ISubscribedParser> parser = await GetRequiredParser(command.ParserId, ct);
        Result<SubscribedParserLink> link = AddLink(parser, command.LinkUrl, command.LinkName);
        return await SaveChanges(link, parser, scope, ct).Map(() => link.Value);
    }

    private async Task<Result> SaveChanges(
        Result<SubscribedParserLink> result,
        Result<ISubscribedParser> parser,
        ITransactionScope txn,
        CancellationToken ct)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (result.IsFailure) return Result.Failure(result.Error);
        await repository.Save(parser.Value);
        return await txn.Commit(ct: ct);
    }
    
    private Result<SubscribedParserLink> AddLink(Result<ISubscribedParser> parser, string linkUrl, string linkName)
    {
        if (parser.IsFailure) return parser.Error;
        SubscribedParserLinkUrlInfo info = SubscribedParserLinkUrlInfo.Create(linkUrl, linkName);
        return parser.Value.AddLink(info);
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(Guid id, CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: id, WithLock: true);
        Result<ISubscribedParser> parser = await repository.Get(query, ct: ct);
        return parser;
    }
}