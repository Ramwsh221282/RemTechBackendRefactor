using Cleaners.Module.Domain;

namespace Cleaners.Module.Cache;

internal interface ICleanerCache
{
    Task Invalidate(ICleaner cleaner);
}
