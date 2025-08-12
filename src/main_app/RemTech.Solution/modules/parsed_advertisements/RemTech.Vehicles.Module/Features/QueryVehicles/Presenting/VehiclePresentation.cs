using System.Data.Common;
using System.Text.Json;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;

public sealed record VehiclePresentation(
    string Id,
    Guid CategoryId,
    Guid BrandId,
    Guid ModelId,
    Guid RegionId,
    long Price,
    bool IsNds,
    string SourceUrl,
    string CategoryName,
    string BrandName,
    string ModelName,
    string City,
    string Region,
    string RegionKind,
    string Description,
    IEnumerable<string> Photos,
    IEnumerable<VehicleCharacteristicsPresentation> Characteristics
)
{
    public static VehiclePresentation FromReader(DbDataReader reader)
    {
        string id = reader.GetString(reader.GetOrdinal("vehicle_id"));
        Guid categoryId = reader.GetGuid(reader.GetOrdinal("category_id"));
        Guid brandId = reader.GetGuid(reader.GetOrdinal("brand_id"));
        Guid modelId = reader.GetGuid(reader.GetOrdinal("model_id"));
        Guid regionId = reader.GetGuid(reader.GetOrdinal("region_id"));
        long price = reader.GetInt64(reader.GetOrdinal("vehicle_price"));
        bool isNds = reader.GetBoolean(reader.GetOrdinal("vehicle_nds"));
        string sourceUrl = reader.GetString(reader.GetOrdinal("vehicle_source_url"));
        string objectData = reader.GetString(reader.GetOrdinal("vehicle_object_data"));
        string description = reader.GetString(reader.GetOrdinal("vehicle_description"));
        JsonDocument jsonDocument = JsonDocument.Parse(objectData);
        string categoryFromJson = CategoryFromJson(jsonDocument);
        string brandFromJson = BrandFromJson(jsonDocument);
        string modelFromJson = ModelFromJson(jsonDocument);
        string regionFromJson = RegionFromJson(jsonDocument);
        string cityFromJson = CityFromJson(jsonDocument);
        string regionKindFromJson = RegionKindFromJson(jsonDocument);
        IEnumerable<string> photosFromJson = PhotosFromJson(jsonDocument);
        IEnumerable<VehicleCharacteristicsPresentation> characteristicsFromJson =
            CharacteristicsFromJson(jsonDocument);
        jsonDocument.Dispose();
        return new VehiclePresentation(
            id,
            categoryId,
            brandId,
            modelId,
            regionId,
            price,
            isNds,
            sourceUrl,
            categoryFromJson,
            brandFromJson,
            modelFromJson,
            cityFromJson,
            regionFromJson,
            regionKindFromJson,
            description,
            photosFromJson,
            characteristicsFromJson
        );
    }

    private static string CategoryFromJson(JsonDocument document)
    {
        return document.RootElement.GetProperty("kind_name").GetString()!;
    }

    private static string BrandFromJson(JsonDocument document)
    {
        return document.RootElement.GetProperty("brand_name").GetString()!;
    }

    private static string ModelFromJson(JsonDocument document)
    {
        return document.RootElement.GetProperty("model_name").GetString()!;
    }

    private static string CityFromJson(JsonDocument document)
    {
        return document.RootElement.GetProperty("location_city").GetString()!;
    }

    private static string RegionFromJson(JsonDocument document)
    {
        return document.RootElement.GetProperty("location_name").GetString()!;
    }

    private static string RegionKindFromJson(JsonDocument document)
    {
        return document.RootElement.GetProperty("location_kind").GetString()!;
    }

    private static IEnumerable<string> PhotosFromJson(JsonDocument document)
    {
        List<string> photos = [];
        foreach (var photo in document.RootElement.GetProperty("photos").EnumerateArray())
            photos.Add(photo.GetProperty("source").GetString()!);
        return photos;
    }

    private static IEnumerable<VehicleCharacteristicsPresentation> CharacteristicsFromJson(
        JsonDocument document
    )
    {
        List<VehicleCharacteristicsPresentation> ctxex = [];
        foreach (
            var characteristic in document
                .RootElement.GetProperty("characteristics")
                .EnumerateArray()
        )
        {
            string name = characteristic.GetProperty("ctx_name").GetString()!;
            string value = characteristic.GetProperty("ctx_value").GetString()!;
            string measure = characteristic.GetProperty("ctx_measure").GetString()!;
            ctxex.Add(new VehicleCharacteristicsPresentation(name, value, measure));
        }
        return ctxex;
    }
}
