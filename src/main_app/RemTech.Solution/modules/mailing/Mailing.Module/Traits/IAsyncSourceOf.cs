using RemTech.Core.Shared.Result;

namespace Mailing.Module.Traits;

public interface IAsyncSourceOf<in TSearchCriteria, TElement> where TSearchCriteria : SearchCriteria
{
    Task<Status<TElement>> Find(TSearchCriteria searchEngine, CancellationToken ct);
}