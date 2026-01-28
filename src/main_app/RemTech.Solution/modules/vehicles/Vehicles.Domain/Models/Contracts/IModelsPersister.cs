using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Models.Contracts;

/// <summary>
/// Контракт для сохранения информации о моделях транспортных средств.
/// </summary>
public interface IModelsPersister
{
	/// <summary>
	/// Сохраняет модель транспортного средства.
	/// </summary>
	/// <param name="model">Модель транспортного средства для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения модели транспортного средства.</returns>
	Task<Result<Model>> Save(Model model, CancellationToken ct = default);
}
