using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Categories.Contracts;

public interface ICategoryPersister
{
    public Task<Result<Category>> Save(Category category, CancellationToken ct = default);
}
