using ParsersControl.Core.Contracts;
using ParsersControl.Core.Extensions;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace ParsersControl.Core.Features.UpdateParserLinks;

[TransactionalHandler]
public sealed class ParserLinkUpdateHandler(ISubscribedParsersRepository repository) : ICommandHandler<UpdateParserLinksCommand, IEnumerable<SubscribedParserLink>>
{
    public async Task<Result<IEnumerable<SubscribedParserLink>>> Execute(UpdateParserLinksCommand command, CancellationToken ct = default)
    {
        Result<SubscribedParser> parser = await GetParser(command.ParserId, ct);
        IEnumerable<ParserLinkUpdater> updaters = GetUpdaters(command);
        Result<IEnumerable<SubscribedParserLink>> result = UpdateLinks(parser, updaters);
        return await SaveChanges(parser, result, ct).Map(() => result.Value);
    }

    private async Task<Result> SaveChanges(Result<SubscribedParser> parser, Result<IEnumerable<SubscribedParserLink>> links, CancellationToken ct)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        if (links.IsFailure) return Result.Failure(links.Error);
        await parser.Value.SaveChanges(repository, ct);
        return Result.Success();
    }
    
    private IEnumerable<ParserLinkUpdater> GetUpdaters(UpdateParserLinksCommand command) =>
        command.UpdateParameters.Select(p =>
            ParserLinkUpdater.Create(
                p.LinkId, 
                p.Activity, 
                p.Name, 
                p.Url).Value);
    
    private async Task<Result<SubscribedParser>> GetParser(Guid id, CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: id);
        return await SubscribedParser.FromRepository(repository, query, ct);
    }

    private Result<IEnumerable<SubscribedParserLink>> UpdateLinks(Result<SubscribedParser> parser, IEnumerable<ParserLinkUpdater> updaters)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.UpdateLinks(updaters);
    }
}