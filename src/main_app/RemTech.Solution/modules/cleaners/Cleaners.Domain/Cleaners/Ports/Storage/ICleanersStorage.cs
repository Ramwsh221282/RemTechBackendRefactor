using Cleaners.Domain.Cleaners.Aggregate;
using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.Ports.Storage;

public interface ICleanersStorage
{
    Task<Status<Cleaner>> Get(CancellationToken ct = default);
    Task<Status<Cleaner>> Get(Guid id, CancellationToken ct = default);
}
