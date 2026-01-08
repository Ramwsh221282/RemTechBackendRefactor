using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParsers;

public sealed class GetParsersQueryHandler(ISubscribedParsersCollectionRepository repository) : IQueryHandler<GetParsersQuery, IEnumerable<ParserResponse>>
{
    private ISubscribedParsersCollectionRepository Repository { get; } = repository;
    
    public async Task<IEnumerable<ParserResponse>> Handle(
        GetParsersQuery query, 
        CancellationToken ct = default)
    {
        SubscribedParsersCollection parsers = await Repository.Get(new SubscribedParsersCollectionQuery(), ct);
        if (parsers.IsEmpty()) return [];
        return parsers.Read().Select(ParserResponse.Create);
    }
}