using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Async;

public sealed class AsyncNewParserLink(INewParserLink inner) : IAsyncNewParserLink
{
    public Task<Status<IParserLink>> AsyncNew(
        AsyncAddParserLink add,
        CancellationToken ct = default
    ) =>
        Task.FromResult(
            inner.Register(new AddParserLink(add.Take(), add.TakeLinkName(), add.TakeLinkUrl()))
        );
}
