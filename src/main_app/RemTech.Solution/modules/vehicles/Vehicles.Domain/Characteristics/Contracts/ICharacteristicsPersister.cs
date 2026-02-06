using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Characteristics.Contracts;

/// <summary>
/// Персистер характеристик.
/// </summary>
public interface ICharacteristicsPersister
{
	/// <summary>
	/// Сохраняет характеристику.
	/// </summary>
	/// <param name="characteristic">Характеристика для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения характеристики.</returns>
	Task<Result<Characteristic>> Save(Characteristic characteristic, CancellationToken ct = default);
}
