using Cleaner.WebApi.Models;

namespace Cleaner.WebApi.Storages;

public interface IWorkingCleanersStorage
{
    Task<WorkingCleaner?> Get();
    Task Invalidate(WorkingCleaner cleaner);
    Task Remove();
}
