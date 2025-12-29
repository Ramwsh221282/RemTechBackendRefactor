using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;
using Vehicles.Domain.Brands;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Characteristics;
using Vehicles.Domain.Contracts;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Models;
using Vehicles.Domain.Vehicles;

namespace Vehicles.Domain.Features.AddVehicle;

[TransactionalHandler]
public sealed class AddVehicleHandler(IPersister persister) : ICommandHandler<AddVehicleCommand, Unit>
{
    public async Task<Result<Unit>> Execute(AddVehicleCommand command, CancellationToken ct = default)
    {
        Brand brand = new Brand(new BrandId(), BrandName.Create(command.Title));
        Category category = new Category(new CategoryId(), CategoryName.Create(command.Title));
        Model model = new Model(new ModelId(), ModelName.Create(command.Title));
        Location location = new Location(new LocationId(), LocationName.Create(command.Address));
        Dictionary<Characteristic, VehicleCharacteristicValue> characteristics = CreateCharacteristicsDictionary(command);
        Dictionary<Characteristic, VehicleCharacteristicValue> savedCharacteristics = await SaveCharacteristics(characteristics, ct);
        IEnumerable<VehicleCharacteristicToAdd> ctxToAdd = savedCharacteristics.Select(kvp => new VehicleCharacteristicToAdd(kvp.Key, kvp.Value));
        
        brand = await brand.SaveBy(persister, ct);
        category = await category.SaveBy(persister, ct);
        model = await model.SaveBy(persister, ct);
        location = await location.SaveBy(persister, ct);
        Vehicle vehicle = CreateVehicle(brand, category, model, location, ctxToAdd, command);
        return Unit.Value;
    }

    private Vehicle CreateVehicle(
        Brand brand, 
        Category category, 
        Model model, 
        Location location,
        IEnumerable<VehicleCharacteristicToAdd> ctxes,
        AddVehicleCommand command)
    {
        VehicleFactory factory = new(brand, category, location, model, ctxes);
        VehicleId id = VehicleId.Create(command.Id);
        VehicleSource source = VehicleSource.Create(command.Url);
        VehiclePriceInformation price = VehiclePriceInformation.Create(command.Price, command.IsNds);
        VehicleTextInformation text = VehicleTextInformation.Create(command.Title);
        IReadOnlyList<VehiclePhoto> photos = command.Photos.Select(p => VehiclePhoto.Create(p).Value).ToList();
        Vehicle vehicle = factory.Construct(id, source, price, text, VehiclePhotosGallery.Create(photos));
        return vehicle;
    }
    
    private Dictionary<Characteristic, VehicleCharacteristicValue> CreateCharacteristicsDictionary(
        AddVehicleCommand command)
    {
        CharacteristicByNameComparer comparer = new();
        Dictionary<Characteristic, VehicleCharacteristicValue> result = new(comparer);
        foreach (AddVehicleCommandCharacteristics characteristic in command.Characteristics)
        {
            Characteristic ctx = new(new CharacteristicId(), CharacteristicName.Create(characteristic.Name));
            VehicleCharacteristicValue value = VehicleCharacteristicValue.Create(characteristic.Value);
            result.Add(ctx, value);
        }
        return result;
    }

    private async Task<Dictionary<Characteristic, VehicleCharacteristicValue>> SaveCharacteristics(
        Dictionary<Characteristic, VehicleCharacteristicValue> existing, 
        CancellationToken ct)
    {
        CharacteristicByNameComparer comparer = new();
        Dictionary<Characteristic, VehicleCharacteristicValue> result = new(comparer);
        foreach (KeyValuePair<Characteristic, VehicleCharacteristicValue> kvp in existing)
        {
            Characteristic ctx = kvp.Key;
            ctx = await ctx.SaveBy(persister, ct);
            result.Add(ctx, kvp.Value);
        }
        return result;
    }

    private sealed class CharacteristicByNameComparer : IEqualityComparer<Characteristic>
    {
        public bool Equals(Characteristic? x, Characteristic? y)
        {
            if (x is null || y is null) return false;
            return x.Name == y.Name;
        }

        public int GetHashCode(Characteristic obj)
        {
            return HashCode.Combine(obj.Name);
        }
    }
}