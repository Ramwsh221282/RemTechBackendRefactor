using ParsedAdvertisements.Domain.ModelContext;
using ParsedAdvertisements.Domain.ModelContext.Ports;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Adapters.Storage.ModelContext;

public sealed class ModelsStorage : IModelsStorage
{
    public Task<Status<Model>> Get(string name, ITransactionManager txn, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Save(Model model, ITransactionManager txn, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}