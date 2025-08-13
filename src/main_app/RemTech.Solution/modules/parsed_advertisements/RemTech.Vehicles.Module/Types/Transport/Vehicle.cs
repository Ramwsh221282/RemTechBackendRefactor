using RemTech.Vehicles.Module.Features.SinkVehicles.Types;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

namespace RemTech.Vehicles.Module.Types.Transport;

internal class Vehicle : IVehicle
{
    protected virtual VehicleIdentity Identity { get; }
    protected virtual SinkedVehicleCategory Category { get; }
    protected virtual SinkedVehicleBrand Brand { get; }
    protected virtual SinkedVehicleLocation Location { get; }
    protected virtual IItemPrice Price { get; }
    protected virtual VehiclePhotos Photos { get; }
    public string SourceUrl { get; private set; }
    public string Sentences { get; private set; }
    public string SourceDomain { get; private set; }
    public virtual VehicleCharacteristics Characteristics { get; }
    protected virtual SinkedVehicleModel Model { get; }

    public string Id() => Identity.Read();

    public Vehicle(
        VehicleIdentity identity,
        IItemPrice price,
        VehiclePhotos photos,
        string sourceUrl,
        string sourceDomain,
        string sentences
    )
    {
        SourceUrl = sourceUrl;
        SourceDomain = sourceDomain;
        Identity = identity;
        Price = price;
        Photos = photos;
        Characteristics = new VehicleCharacteristics([]);
        Category = new SinkedVehicleCategory(string.Empty, Guid.Empty);
        Brand = new SinkedVehicleBrand(string.Empty, Guid.Empty);
        Location = new SinkedVehicleLocation(string.Empty, string.Empty, string.Empty, Guid.Empty);
        Model = new SinkedVehicleModel(string.Empty, Guid.Empty);
        Sentences = sentences;
    }

    public string MakeDocument()
    {
        string[] parts =
        [
            Category.Name,
            Brand.Name,
            Model.Name,
            Characteristics.MakeDocument(),
            Sentences,
            $"{Location.CityText} {Location.Text} {Location.KindText}",
        ];
        return string.Join(' ', parts);
    }

    public Vehicle Accept(IEnumerable<VehicleCharacteristic> characteristics) =>
        new(this, new VehicleCharacteristics(characteristics));

    public Vehicle Accept(VehicleCharacteristic characteristic) => new(this, characteristic);

    public Vehicle Accept(VehicleCharacteristics characteristics) => new(this, characteristics);

    public Vehicle Accept(SinkedVehicleCategory category) => new(this, category);

    public Vehicle Accept(SinkedVehicleBrand brand) => new(this, brand);

    public Vehicle Accept(SinkedVehicleLocation location) => new(this, location);

    public Vehicle Accept(SinkedVehicleModel vehicleModel) => new(this, vehicleModel);

    private Vehicle(Vehicle origin, VehicleCharacteristics characteristics)
        : this(origin) => Characteristics = characteristics;

    private Vehicle(Vehicle origin, IEnumerable<VehicleCharacteristic> characteristics)
        : this(origin) => Characteristics = new VehicleCharacteristics(characteristics);

    private Vehicle(Vehicle origin, SinkedVehicleCategory category)
        : this(origin) => Category = category;

    private Vehicle(Vehicle origin, SinkedVehicleBrand brand)
        : this(origin) => Brand = brand;

    private Vehicle(Vehicle origin, SinkedVehicleLocation location)
        : this(origin) => Location = location;

    private Vehicle(Vehicle origin, SinkedVehicleModel model)
        : this(origin) => Model = model;

    private Vehicle(Vehicle origin, VehicleCharacteristic ctx)
        : this(origin)
    {
        VehicleCharacteristic[] current = origin.Characteristics.Read();
        Characteristics = new VehicleCharacteristics([.. current, ctx]);
    }

    public Vehicle(Vehicle origin)
    {
        Identity = origin.Identity;
        Category = origin.Category;
        Brand = origin.Brand;
        Location = origin.Location;
        Price = origin.Price;
        Photos = origin.Photos;
        Characteristics = origin.Characteristics;
        Model = origin.Model;
        SourceUrl = origin.SourceUrl;
        SourceDomain = origin.SourceDomain;
        Sentences = origin.Sentences;
    }
}
