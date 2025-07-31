using System.Data.Common;
using System.Text.Json;
using RemTech.Vehicles.Module.Features.QueryVehicleBrands.Types;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;
using RemTech.Vehicles.Module.Features.QueryVehicleModels.Types;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles;

public sealed class VehiclePresentsReader : IAsyncDisposable
{
    private readonly DbDataReader _reader;

    public VehiclePresentsReader(DbDataReader reader)
    {
        _reader = reader;
    }

    public async Task<IEnumerable<VehiclePresentation>> Read(CancellationToken ct = default)
    {
        List<VehiclePresentation> results = [];
        while (await _reader.ReadAsync(ct))
        {
            string objectData = _reader.GetString(_reader.GetOrdinal("object_data"));
            using JsonDocument document = JsonDocument.Parse(objectData);
            VehicleIdentityPresentation identity = ParseIdentity();
            VehiclePricePresentation price = ParsePrice();
            VehicleBrandPresentation brand = ParseBrand(document);
            VehicleModelPresentation model = ParseModel(document);
            VehicleKindPresentation kind = ParseKind(document);
            VehicleRegionPresentation region = ParseRegion(document);
            VehiclePhotosPresentation photos = ParsePhotos(document);
            VehicleCharacteristicsPresentation characteristics = ParseCharacteristics(document);
            VehiclePresentation presentation = new(
                identity,
                kind,
                brand,
                model,
                region,
                price,
                photos,
                characteristics
            );
            results.Add(presentation);
        }
        return results;
    }

    private VehicleIdentityPresentation ParseIdentity()
    {
        string id = _reader.GetString(_reader.GetOrdinal("vehicle_id"));
        VehicleIdentityPresentation identity = new(id);
        return identity;
    }

    private VehiclePricePresentation ParsePrice()
    {
        double price = _reader.GetDouble(_reader.GetOrdinal("vehicle_price"));
        bool isNds = _reader.GetBoolean(_reader.GetOrdinal("vehicle_nds"));
        return new VehiclePricePresentation(price, isNds);
    }

    private VehicleBrandPresentation ParseBrand(JsonDocument document)
    {
        Guid brandId = _reader.GetGuid(_reader.GetOrdinal("brand_id"));
        string brandName = document.RootElement.GetProperty("brand_name").GetString()!;
        return new VehicleBrandPresentation(brandId, brandName);
    }

    private VehicleModelPresentation ParseModel(JsonDocument document)
    {
        Guid modelId = _reader.GetGuid(_reader.GetOrdinal("model_id"));
        string modelName = document.RootElement.GetProperty("model_name").GetString()!;
        return new VehicleModelPresentation(modelId, modelName);
    }

    private VehicleKindPresentation ParseKind(JsonDocument document)
    {
        Guid kindId = _reader.GetGuid(_reader.GetOrdinal("kind_id"));
        string kindName = document.RootElement.GetProperty("kind_name").GetString()!;
        return new VehicleKindPresentation(kindId, kindName);
    }

    private VehicleRegionPresentation ParseRegion(JsonDocument document)
    {
        Guid regionId = _reader.GetGuid(_reader.GetOrdinal("geo_id"));
        string regionName = document.RootElement.GetProperty("location_name").GetString()!;
        string regionKind = document.RootElement.GetProperty("location_kind").GetString()!;
        return new VehicleRegionPresentation(regionId, regionName, regionKind);
    }

    private VehiclePhotosPresentation ParsePhotos(JsonDocument document)
    {
        JsonElement photosArray = document.RootElement.GetProperty("photos");
        VehiclePhotoPresentation[] photos = new VehiclePhotoPresentation[
            photosArray.GetArrayLength()
        ];
        int index = 0;
        foreach (JsonElement photo in photosArray.EnumerateArray())
        {
            string source = photo.GetProperty("source").GetString()!;
            photos[index] = new VehiclePhotoPresentation(source);
            index++;
        }
        return new VehiclePhotosPresentation(photos);
    }

    private VehicleCharacteristicsPresentation ParseCharacteristics(JsonDocument document)
    {
        JsonElement characteristics = document.RootElement.GetProperty("characteristics");
        int length = characteristics.GetArrayLength();
        int index = 0;
        VehicleCharacteristicPresentation[] ctx = new VehicleCharacteristicPresentation[length];
        foreach (JsonElement entry in characteristics.EnumerateArray())
        {
            string name = entry.GetProperty("ctx_name").GetString()!;
            string value = entry.GetProperty("ctx_value").GetString()!;
            string measure = entry.GetProperty("ctx_measure").GetString()!;
            ctx[index] = new VehicleCharacteristicPresentation(name, value, measure);
            index++;
        }

        return new VehicleCharacteristicsPresentation(ctx);
    }

    public async ValueTask DisposeAsync()
    {
        await _reader.DisposeAsync();
    }
}
