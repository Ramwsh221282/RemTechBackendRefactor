using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.AddParserLink;

[TransactionalHandler]
public sealed class AddParserLinkHandler(ISubscribedParsersRepository repository) 
    : ICommandHandler<AddParserLinkCommand, IEnumerable<SubscribedParserLink>>
{
    public async Task<Result<IEnumerable<SubscribedParserLink>>> Execute(
        AddParserLinkCommand command, 
        CancellationToken ct = default)
    {
        Result<SubscribedParser> parser = await GetRequiredParser(command.ParserId, ct);
        Result<IEnumerable<SubscribedParserLink>> links = AddLinks(parser, command.Links);
        return await SaveChanges(links, parser, ct).Map(() => links.Value);
    }

    private async Task<Result> SaveChanges(
        Result<IEnumerable<SubscribedParserLink>> result, 
        Result<SubscribedParser> parser,
        CancellationToken ct)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (result.IsFailure) return Result.Failure(result.Error);
        await parser.Value.SaveChanges(repository, ct);
        return Result.Success();
    }
    
    private Result<IEnumerable<SubscribedParserLink>> AddLinks(Result<SubscribedParser> parser, IEnumerable<AddParserLinkCommandArg> args)
    {
        if (parser.IsFailure) return parser.Error;
        IEnumerable<SubscribedParserLinkUrlInfo> infos = args
            .Select(arg => SubscribedParserLinkUrlInfo.Create(arg.LinkUrl, arg.LinkName).Value);
        return parser.Value.AddLinks(infos);
    }
    
    private async Task<Result<SubscribedParser>> GetRequiredParser(Guid id, CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: id, WithLock: true);
        return await repository.Get(query, ct: ct);
    }
}