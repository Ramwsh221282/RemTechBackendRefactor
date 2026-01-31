using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;
using Vehicles.Domain.Brands;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Characteristics;
using Vehicles.Domain.Contracts;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Models;
using Vehicles.Domain.Vehicles;
using Vehicles.Domain.Vehicles.Contracts;

namespace Vehicles.Domain.Features.AddVehicle;

/// <summary>
/// Обработчик команды добавления транспортного средства.
/// </summary>
/// <param name="persister">Персистер для сохранения данных.</param>
[TransactionalHandler]
public sealed class AddVehicleHandler(IPersister persister) : ICommandHandler<AddVehicleCommand, (Guid, int)>
{
	/// <summary>
	/// Выполняет обработку команды добавления транспортного средства.
	/// </summary>
	/// <param name="command">Команда добавления транспортного средства.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды, содержащий идентификатор создателя и количество добавленных транспортных средств.</returns>
	public async Task<Result<(Guid, int)>> Execute(AddVehicleCommand command, CancellationToken ct = default)
	{
		List<VehiclePersistInfo> vehiclesToSave = [];
		foreach (AddVehicleVehiclesCommandPayload payload in command.Vehicles)
		{
			Result<Brand> brand = CreateValidBrand(payload);
			if (brand.IsFailure)
				continue;

			Result<Category> category = CreateValidCategory(payload);
			if (category.IsFailure)
				continue;

			Result<Model> model = CreateValidModel(payload);
			if (model.IsFailure)
				continue;

			Result<Location> location = CreateValidLocation(payload);
			if (location.IsFailure)
				continue;

			Dictionary<Characteristic, VehicleCharacteristicValue> characteristics = CreateCharacteristicsDictionary(
				payload
			);
			Dictionary<Characteristic, VehicleCharacteristicValue> savedCharacteristics = await SaveCharacteristics(
				characteristics,
				ct
			);
			IEnumerable<VehicleCharacteristicToAdd> ctxToAdd = savedCharacteristics.Select(
				kvp => new VehicleCharacteristicToAdd(kvp.Key, kvp.Value)
			);

			Result<Brand> savedBrand = await brand.Value.SaveBy(persister, ct);
			if (savedBrand.IsFailure)
				continue;

			Result<Category> savedCategory = await category.Value.SaveBy(persister, ct);
			if (savedCategory.IsFailure)
				continue;

			Result<Model> savedModel = await model.Value.SaveBy(persister, ct);
			if (savedModel.IsFailure)
				continue;

			Result<Location> savedLocation = await location.Value.SaveBy(persister, ct);
			if (savedLocation.IsFailure)
				continue;

			Vehicle vehicle = CreateVehicle(
				savedBrand.Value,
				savedCategory.Value,
				savedModel.Value,
				savedLocation.Value,
				ctxToAdd,
				payload
			);
			VehiclePersistInfo persistInfo = new(vehicle, savedLocation.Value);
			vehiclesToSave.Add(persistInfo);
		}

		int saved = await persister.Save(vehiclesToSave, ct);
		return (command.Creator.CreatorId, saved);
	}

	private static Result<Model> CreateValidModel(AddVehicleVehiclesCommandPayload payload)
	{
		Result<ModelName> modelName = ModelName.Create(payload.Title);
		return modelName.IsFailure ? modelName.Error : new Model(ModelId.Create(Guid.NewGuid()), modelName);
	}

	private static Result<Location> CreateValidLocation(AddVehicleVehiclesCommandPayload payload)
	{
		Result<LocationName> locationName = LocationName.Create(payload.Address);
		return locationName.IsFailure
			? (Result<Location>)locationName.Error
			: new Location(LocationId.Create(Guid.NewGuid()), locationName);
	}

	private static Result<Brand> CreateValidBrand(AddVehicleVehiclesCommandPayload payload)
	{
		Result<BrandName> brandName = BrandName.Create(payload.Title);
		return brandName.IsFailure
			? (Result<Brand>)brandName.Error
			: (Result<Brand>)new Brand(BrandId.Create(Guid.NewGuid()), brandName);
	}

	private static Result<Category> CreateValidCategory(AddVehicleVehiclesCommandPayload payload)
	{
		Result<CategoryName> categoryName = CategoryName.Create(payload.Title);
		return categoryName.IsFailure
			? (Result<Category>)categoryName.Error
			: (Result<Category>)new Category(CategoryId.Create(Guid.NewGuid()), categoryName);
	}

	private static Vehicle CreateVehicle(
		Brand brand,
		Category category,
		Model model,
		Location location,
		IEnumerable<VehicleCharacteristicToAdd> ctxes,
		AddVehicleVehiclesCommandPayload payload
	)
	{
		VehicleFactory factory = new(brand, category, location, model, ctxes);
		VehicleId id = VehicleId.Create(payload.Id);
		VehicleSource source = VehicleSource.Create(payload.Url);
		VehiclePriceInformation price = VehiclePriceInformation.Create(payload.Price, payload.IsNds);
		VehicleTextInformation text = VehicleTextInformation.Create(payload.Title);
		IReadOnlyList<VehiclePhoto> photos = [.. payload.Photos.Select(p => VehiclePhoto.Create(p).Value)];
		return factory.Construct(id, source, price, text, VehiclePhotosGallery.Create(photos));
	}

	private static Dictionary<Characteristic, VehicleCharacteristicValue> CreateCharacteristicsDictionary(
		AddVehicleVehiclesCommandPayload payload
	)
	{
		CharacteristicByNameComparer comparer = new();
		Dictionary<Characteristic, VehicleCharacteristicValue> result = new(comparer);
		foreach (AddVehicleCommandCharacteristics characteristic in payload.Characteristics)
		{
			Characteristic ctx = new(
				CharacteristicId.Create(Guid.NewGuid()),
				CharacteristicName.Create(characteristic.Name)
			);
			if (result.ContainsKey(ctx))
				continue;
			VehicleCharacteristicValue value = VehicleCharacteristicValue.Create(characteristic.Value);
			result.Add(ctx, value);
		}

		return result;
	}

	private async Task<Dictionary<Characteristic, VehicleCharacteristicValue>> SaveCharacteristics(
		Dictionary<Characteristic, VehicleCharacteristicValue> existing,
		CancellationToken ct
	)
	{
		CharacteristicByNameComparer comparer = new();
		Dictionary<Characteristic, VehicleCharacteristicValue> result = new(comparer);

		foreach (KeyValuePair<Characteristic, VehicleCharacteristicValue> kvp in existing)
		{
			Characteristic ctx = kvp.Key;
			ctx = await ctx.SaveBy(persister, ct);
			if (result.ContainsKey(ctx))
				continue;
			result.Add(ctx, kvp.Value);
		}

		return result;
	}

	private sealed class CharacteristicByNameComparer : IEqualityComparer<Characteristic>
	{
		public bool Equals(Characteristic? x, Characteristic? y)
		{
			return x is null || y is null ? false : x.Name == y.Name;
		}

		public int GetHashCode(Characteristic obj) => HashCode.Combine(obj.Name);
	}
}
