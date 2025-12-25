using ParsersControl.Core.Contracts;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.Features.SetLinkParsedAmount;

public sealed class SetLinkParsedAmountHandler(
    ITransactionSource transactionSource, 
    ISubscribedParsersRepository parsers)
    : ICommandHandler<SetLinkParsedAmountCommand, SubscribedParserLink>
{
    public async Task<Result<SubscribedParserLink>> Execute(
        SetLinkParsedAmountCommand command, 
        CancellationToken ct = default)
    {
        ITransactionScope scope = await transactionSource.BeginTransaction(ct);
        Result<ISubscribedParser> parser = await GetRequiredParser(command.ParserId, ct);
        Result<SubscribedParserLink> link = SetLinkParsedAmount(parser, command.LinkId, command.Amount);
        return await SaveChanges(parser, link, scope, ct).Map(() => link.Value);
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser,
        Result<SubscribedParserLink> link,
        ITransactionScope txn,
        CancellationToken ct
    )
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (link.IsFailure) return Result.Failure(link.Error);
        await parsers.Save(parser.Value);
        return await txn.Commit(ct);
    }
    
    private Result<SubscribedParserLink> SetLinkParsedAmount(
        Result<ISubscribedParser> parser, 
        Guid linkId,
        int amount)
    {
        if (parser.IsFailure) return parser.Error;
        Result<SubscribedParserLink> link = parser.Value.FindLink(linkId);
        if (link.IsFailure) return link.Error;
        return parser.Value.AddLinkParsedAmount(link.Value, amount);
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(Guid parserId, CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: parserId, WithLock: true);
        return await parsers.Get(query, ct);
    }
}