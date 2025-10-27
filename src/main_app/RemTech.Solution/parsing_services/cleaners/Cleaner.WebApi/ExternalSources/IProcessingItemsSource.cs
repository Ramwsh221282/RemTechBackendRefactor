using Cleaner.WebApi.Models;

namespace Cleaner.WebApi.ExternalSources;

public interface IProcessingItemsSource
{
    Task<IEnumerable<CleanerProcessingItem>> GetItemsForCleaner(
        WorkingCleaner cleaner,
        int batchSize
    );
}
