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
			{
				continue;
			}

			Result<Category> category = CreateValidCategory(payload);
			if (category.IsFailure)
			{
				continue;
			}

			Result<Model> model = CreateValidModel(payload);
			if (model.IsFailure)
			{
				continue;
			}

			Result<Location> location = CreateValidLocation(payload);
			if (location.IsFailure)
			{
				continue;
			}

			Dictionary<Characteristic, VehicleCharacteristicValue> characteristics = CreateCharacteristicsDictionary(
				payload
			);

            IReadOnlyList<VehicleCharacteristicToAdd> savedCharacteristics = await SaveCharacteristics(characteristics, ct);

			Result<Brand> savedBrand = await brand.Value.SaveBy(persister, ct);
			if (savedBrand.IsFailure)
			{
				continue;
			}

			Result<Category> savedCategory = await category.Value.SaveBy(persister, ct);
			if (savedCategory.IsFailure)
			{
				continue;
			}

			Result<Model> savedModel = await model.Value.SaveBy(persister, ct);
			if (savedModel.IsFailure)
			{
				continue;
			}

			Result<Location> savedLocation = await location.Value.SaveBy(persister, ct);
			if (savedLocation.IsFailure)
			{
				continue;
			}

			Vehicle vehicle = CreateVehicle(
				savedBrand.Value,
				savedCategory.Value,
				savedModel.Value,
				savedLocation.Value,
				savedCharacteristics,
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
			? locationName.Error
			: new Location(LocationId.Create(Guid.NewGuid()), locationName);
	}

	private static Result<Brand> CreateValidBrand(AddVehicleVehiclesCommandPayload payload)
	{
		Result<BrandName> brandName = BrandName.Create(payload.Title);
		return brandName.IsFailure
			? brandName.Error
			: new Brand(BrandId.Create(Guid.NewGuid()), brandName);
	}

	private static Result<Category> CreateValidCategory(AddVehicleVehiclesCommandPayload payload)
	{
		Result<CategoryName> categoryName = CategoryName.Create(payload.Title);
		return categoryName.IsFailure
			? categoryName.Error
			: new Category(CategoryId.Create(Guid.NewGuid()), categoryName);
	}

	private static Result<Vehicle> CreateVehicle(
		Brand brand,
		Category category,
		Model model,
		Location location,
		IEnumerable<VehicleCharacteristicToAdd> ctxes,
		AddVehicleVehiclesCommandPayload payload
	)
	{
		VehicleFactory factory = new(brand, category, location, model, ctxes);
        Result<VehicleId> id = VehicleId.Create(payload.Id);
		if (id.IsFailure)
		{
			return id.Error;
		}

        Result<VehicleSource> source = VehicleSource.Create(payload.Url);
		if (source.IsFailure)
		{
			return source.Error;
		}

        Result<VehiclePriceInformation> price = VehiclePriceInformation.Create(payload.Price, payload.IsNds);
		if (price.IsFailure)
		{
			return price.Error;
		}
		
        Result<VehicleTextInformation> text = VehicleTextInformation.Create(payload.Title);
		if (text.IsFailure)
		{
			return text.Error;
		}

		List<VehiclePhoto> photos = [];
		foreach (string photo in payload.Photos)
		{
			if (string.IsNullOrWhiteSpace(photo))
			{
				continue;
			}

            Result<VehiclePhoto> photoRes = VehiclePhoto.Create(photo);
			if (photoRes.IsFailure)
			{
				continue;
			}			

			photos.Add(photoRes.Value);
		}		

		return factory.Construct(id, source, price, text, VehiclePhotosGallery.Create(photos));	
	}

	private static Dictionary<Characteristic, VehicleCharacteristicValue> CreateCharacteristicsDictionary(
		AddVehicleVehiclesCommandPayload payload
	)
	{
		Dictionary<Characteristic, VehicleCharacteristicValue> result = new(CharacteristicByNameComparer);
        
		foreach (AddVehicleCommandCharacteristics characteristic in payload.Characteristics)
		{
            Result<CharacteristicName> name = CharacteristicName.Create(characteristic.Name);
            if (name.IsFailure)
            {
                continue;
            }
            
            CharacteristicId id = CharacteristicId.Create(Guid.NewGuid());
			Characteristic ctx = new(id, name);
			if (result.ContainsKey(ctx))
			{
				continue;
			}

			VehicleCharacteristicValue value = VehicleCharacteristicValue.Create(characteristic.Value);
			result.Add(ctx, value);
		}

		return result;
	}

	private async Task<IReadOnlyList<VehicleCharacteristicToAdd>> SaveCharacteristics(
		Dictionary<Characteristic, VehicleCharacteristicValue> existing,
		CancellationToken ct
	)
    {
        Dictionary<Characteristic, VehicleCharacteristicToAdd> result = new(CharacteristicByNameComparer);
		foreach (KeyValuePair<Characteristic, VehicleCharacteristicValue> kvp in existing)
		{
			Characteristic ctx = kvp.Key;
            
            Result<Characteristic> resolved = await ctx.SaveBy(persister, ct);
            if (resolved.IsFailure)
            {
                continue;
            }

            if (result.ContainsKey(resolved.Value))
            {
                continue;
            }

            Characteristic existingCharacterstic = new(resolved.Value.Id, resolved.Value.Name);
            result.Add(resolved.Value, new VehicleCharacteristicToAdd(existingCharacterstic, kvp.Value));
		}

        return result.Select(r => r.Value).ToList();
    }

    private static readonly EqualityComparer<Characteristic> CharacteristicByNameComparer =
        EqualityComparer<Characteristic>.Create(NamesEqual, HashCodeForNameComparison);

    private static bool NamesEqual(Characteristic? first, Characteristic? second)
    {
        if (first == null)
        {
            return false;
        }

        if (second is null)
        {
            return false;
        }

        return second.Name == first.Name;
    }
    
    private static int HashCodeForNameComparison(Characteristic? ctx)
    {
        return ctx is null ? 0 : ctx.Name.GetHashCode();
    }
}
