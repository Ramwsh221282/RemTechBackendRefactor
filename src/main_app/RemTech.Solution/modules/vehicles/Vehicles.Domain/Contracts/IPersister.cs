using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Brands;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Characteristics;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Models;
using Vehicles.Domain.Vehicles.Contracts;

namespace Vehicles.Domain.Contracts;

/// <summary>
/// Персистер сущностей транспортных средств.
/// </summary>
public interface IPersister
{
	/// <summary>
	/// Сохраняет бренд.
	/// </summary>
	/// <param name="brand">Бренд для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения бренда.</returns>
	Task<Result<Brand>> Save(Brand brand, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет модель.
	/// </summary>
	/// <param name="model">Модель для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения модели.</returns>
	Task<Result<Model>> Save(Model model, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет локацию.
	/// </summary>
	/// <param name="location">Локация для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения локации.</returns>
	Task<Result<Location>> Save(Location location, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет категорию.
	/// </summary>
	/// <param name="category">Категория для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения категории.</returns>
	Task<Result<Category>> Save(Category category, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет характеристику.
	/// </summary>
	/// <param name="characteristic">Характеристика для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения характеристики.</returns>
	Task<Result<Characteristic>> Save(Characteristic characteristic, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет информацию о персистенции транспортного средства.
	/// </summary>
	/// <param name="info">Информация о персистенции транспортного средства для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения информации о персистенции транспортного средства.</returns>
	Task<Result<VehiclePersistInfo>> Save(VehiclePersistInfo info, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет несколько информации о персистенции транспортных средств.
	/// </summary>
	/// <param name="infos">Коллекция информации о персистенции транспортных средств для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Количество успешно сохраненных записей.</returns>
	Task<int> Save(IEnumerable<VehiclePersistInfo> infos, CancellationToken ct = default);
}
