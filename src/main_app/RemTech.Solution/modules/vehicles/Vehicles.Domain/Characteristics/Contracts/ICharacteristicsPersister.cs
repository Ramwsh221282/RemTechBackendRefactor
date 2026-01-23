using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Characteristics.Contracts;

public interface ICharacteristicsPersister
{
	Task<Result<Characteristic>> Save(Characteristic characteristic, CancellationToken ct = default);
}
