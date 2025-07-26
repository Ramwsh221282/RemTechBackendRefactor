using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.PresentInfos;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation;

public sealed class VehiclePresent
{
    private readonly string _id;
    private readonly VehiclePresentPriceInfo _priceInfo;
    private readonly VehiclePresentKindInfo _kind;
    private readonly VehiclePresentBrandInfo _brand;
    private readonly VehiclePresentModelInfo _model;
    private readonly VehiclePresentLocationInfo _location;
    private readonly VehiclePresentPhotosInfo _photos;
    private readonly VehiclePresentCharacteristicsCollection _characteristics;

    public VehiclePresent()
    {
        _id = string.Empty;
        _priceInfo = new VehiclePresentPriceInfo();
        _kind = new VehiclePresentKindInfo();
        _brand = new VehiclePresentBrandInfo();
        _model = new VehiclePresentModelInfo();
        _location = new VehiclePresentLocationInfo();
        _photos = new VehiclePresentPhotosInfo();
        _characteristics = new VehiclePresentCharacteristicsCollection();
    }

    private VehiclePresent(
        string id,
        VehiclePresentPriceInfo priceInfo,
        VehiclePresentKindInfo kind,
        VehiclePresentBrandInfo brand,
        VehiclePresentModelInfo model,
        VehiclePresentLocationInfo location,
        VehiclePresentPhotosInfo photos,
        VehiclePresentCharacteristicsCollection characteristics
    )
    {
        _id = id;
        _priceInfo = priceInfo;
        _kind = kind;
        _brand = brand;
        _model = model;
        _location = location;
        _photos = photos;
        _characteristics = characteristics;
    }

    public VehiclePresent RiddenBy(DbDataReader reader)
    {
        return new VehiclePresent(
            reader.GetString(reader.GetOrdinal("vehicle_id")),
            _priceInfo.RiddenBy(reader),
            _kind.RiddenBy(reader),
            _brand.RiddenBy(reader),
            _model.RiddenBy(reader),
            _location.RiddenBy(reader),
            _photos.RiddenBy(reader),
            _characteristics.RiddenBy(reader)
        );
    }
}