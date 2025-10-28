using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.ModelContext.Ports;

public interface IModelsStorage
{
    Task<Status<Model>> Get(string name, ITransactionManager txn, CancellationToken ct = default);
    Task Save(Model model, ITransactionManager txn, CancellationToken ct = default);
}