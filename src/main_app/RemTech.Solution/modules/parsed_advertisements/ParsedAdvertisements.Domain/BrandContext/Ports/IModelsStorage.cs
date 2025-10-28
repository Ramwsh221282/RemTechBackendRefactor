using ParsedAdvertisements.Domain.ModelContext;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.BrandContext.Ports;

public interface IModelsStorage
{
    Task<Status<Model>> Get(string name, CancellationToken ct = default);
    Task Save(Model model, ITransaction txn, CancellationToken ct = default);
}
