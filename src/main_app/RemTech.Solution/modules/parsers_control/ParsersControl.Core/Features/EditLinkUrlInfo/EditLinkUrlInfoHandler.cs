using ParsersControl.Core.Contracts;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.Features.EditLinkUrlInfo;

public sealed class EditLinkUrlInfoHandler(
    ITransactionSource source, 
    ISubscribedParsersRepository repository)
    : ICommandHandler<EditLinkUrlInfoCommand, SubscribedParserLink>
{
    public async Task<Result<SubscribedParserLink>> Execute(EditLinkUrlInfoCommand command, CancellationToken ct = default)
    {
        ITransactionScope scope = await source.BeginTransaction(ct);
        Result<ISubscribedParser> parser = await GetRequiredParser(command.ParserId, ct);
        Result<SubscribedParserLink> link = EditLink(parser, command.LinkId, command.NewName, command.NewUrl);
        return await SaveChanges(parser, link, scope, ct).Map(() => link.Value);
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(Guid parserId, CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: parserId, WithLock: true);
        return await repository.Get(query);
    }

    private Result<SubscribedParserLink> EditLink(
        Result<ISubscribedParser> parser,
        Guid linkId,
        string? newName,
        string? newUrl
    )
    {
        if (parser.IsFailure) return Result.Failure<SubscribedParserLink>(parser.Error);
        Result<SubscribedParserLink> link = parser.Value.FindLink(linkId);
        if (link.IsFailure) return Result.Failure<SubscribedParserLink>(link.Error);
        Result<SubscribedParserLink> editResult = parser.Value.EditLink(link.Value, newName, newUrl);
        if (editResult.IsFailure) return Result.Failure<SubscribedParserLink>(editResult.Error);
        return editResult;
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser,
        Result<SubscribedParserLink> editResult,
        ITransactionScope scope,
        CancellationToken ct)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (editResult.IsFailure) return Result.Failure(editResult.Error);
        await repository.Save(parser.Value);
        return await scope.Commit(ct);
    }
}