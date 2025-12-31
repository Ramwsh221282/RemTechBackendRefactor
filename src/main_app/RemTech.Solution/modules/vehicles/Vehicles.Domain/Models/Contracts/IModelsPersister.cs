using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Models.Contracts;

public interface IModelsPersister
{
    Task<Result<Model>> Save(Model model, CancellationToken ct = default);
}