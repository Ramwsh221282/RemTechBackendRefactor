using Cleaners.Domain.Cleaners.Aggregate;
using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.Ports.Cache;

public interface ICleanersCachedStorage
{
    Task<Status<Cleaner>> Get(Guid id);
    Task Invalidate(Cleaner cleaner);
}
