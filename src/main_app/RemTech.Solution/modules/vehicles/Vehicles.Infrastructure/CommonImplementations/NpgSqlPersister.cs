using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Brands;
using Vehicles.Domain.Brands.Contracts;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Categories.Contracts;
using Vehicles.Domain.Characteristics;
using Vehicles.Domain.Characteristics.Contracts;
using Vehicles.Domain.Contracts;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Locations.Contracts;
using Vehicles.Domain.Models;
using Vehicles.Domain.Models.Contracts;
using Vehicles.Domain.Vehicles.Contracts;

namespace Vehicles.Infrastructure.CommonImplementations;

/// <summary>
/// Реализация персистера для PostgreSQL.
/// </summary>
/// <param name="brandPersister">Персистер для работы с брендами.</param>
/// <param name="modelPersister">Персистер для работы с моделями.</param>
/// <param name="categoryPersister">Персистер для работы с категориями.</param>
/// <param name="characteristicPersister">Персистер для работы с характеристиками.</param>
/// <param name="locationsPersister">Персистер для работы с локациями.</param>
/// <param name="vehiclesPersister">Персистер для работы с транспортными средствами.</param>
/// <param name="vehiclesListPersister">Персистер для работы с списком транспортных средств.</param>
public sealed class NpgSqlPersister(
	IBrandPersister brandPersister,
	IModelsPersister modelPersister,
	ICategoryPersister categoryPersister,
	ICharacteristicsPersister characteristicPersister,
	ILocationsPersister locationsPersister,
	IVehiclesPersister vehiclesPersister,
	IVehiclesListPersister vehiclesListPersister
) : IPersister
{
	/// <summary>
	/// Сохраняет бренд.
	/// </summary>
	/// <param name="brand">Бренд для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции сохранения бренда.</returns>
	public Task<Result<Brand>> Save(Brand brand, CancellationToken ct = default)
	{
		return brandPersister.Save(brand, ct);
	}

	/// <summary>
	/// Сохраняет модель.
	/// </summary>
	/// <param name="model">Модель для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции сохранения модели.</returns>
	public Task<Result<Model>> Save(Model model, CancellationToken ct = default)
	{
		return modelPersister.Save(model, ct);
	}

	/// <summary>
	/// Сохраняет локацию.
	/// </summary>
	/// <param name="location">Локация для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции сохранения локации.</returns>
	public Task<Result<Location>> Save(Location location, CancellationToken ct = default)
	{
		return locationsPersister.Save(location, ct);
	}

	/// <summary>
	/// Сохраняет категорию.
	/// </summary>
	/// <param name="category">Категория для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции сохранения категории.</returns>
	public Task<Result<Category>> Save(Category category, CancellationToken ct = default)
	{
		return categoryPersister.Save(category, ct);
	}

	/// <summary>
	/// Сохраняет характеристику.
	/// </summary>
	/// <param name="characteristic">Характеристика для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции сохранения характеристики.</returns>
	public Task<Result<Characteristic>> Save(Characteristic characteristic, CancellationToken ct = default)
	{
		return characteristicPersister.Save(characteristic, ct);
	}

	/// <summary>
	/// Сохраняет информацию о транспортном средстве.
	/// </summary>
	/// <param name="info">Информация о транспортном средстве для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции сохранения информации о транспортном средстве.</returns>
	public async Task<Result<VehiclePersistInfo>> Save(VehiclePersistInfo info, CancellationToken ct = default)
	{
		Result<Unit> result = await vehiclesPersister.Persist(info, ct);
		return result.IsSuccess ? info : result.Error;
	}

	/// <summary>
	/// Сохраняет несколько транспортных средств.
	/// </summary>
	/// <param name="infos">Список информации о транспортных средствах для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции сохранения нескольких транспортных средств.</returns>
	public Task<int> Save(IEnumerable<VehiclePersistInfo> infos, CancellationToken ct = default)
	{
		return vehiclesListPersister.Persist(infos, ct);
	}
}
