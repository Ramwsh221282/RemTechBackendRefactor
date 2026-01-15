using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParsers;

public sealed class GetParsersQueryHandler(ISubscribedParsersCollectionRepository repository)
    : IQueryHandler<GetParsersQuery, IEnumerable<ParserResponse>>
{
    private ISubscribedParsersCollectionRepository Repository { get; } = repository;

    public async Task<IEnumerable<ParserResponse>> Handle(
        GetParsersQuery query,
        CancellationToken ct = default
    )
    {
        SubscribedParsersCollectionQuery emptyQuery = new();
        SubscribedParsersCollection parsers = await Repository.Get(emptyQuery, ct);
        return parsers.IsEmpty() ? [] : parsers.Read().Select(ParserResponse.Create);
    }
}
