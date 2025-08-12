using System.Data.Common;
using System.Text.Json;

namespace RemTech.ContainedItems.Module.Features.QueryRecentContainedItems;

internal sealed record RecentVehicle : SomeRecentItem
{
    public Guid CategoryId { get; }
    public Guid BrandId { get; }
    public Guid ModelId { get; }
    public Guid RegionId { get; }
    public string CategoryName { get; }
    public string BrandName { get; }
    public string ModelName { get; }

    private const string VehicleIdColumn = "vehicle_id";
    private const string VehiclePriceColumn = "vehicle_price";
    private const string VehicleNdsColumn = "vehicle_nds";
    private const string BrandIdColumn = "brand_id";
    private const string KindIdColumn = "kind_id";
    private const string ModelIdColumn = "model_id";
    private const string GeoIdColumn = "geo_id";
    private const string DescriptionColumn = "vehicle_description";
    private const string SourceUrlColumn = "vehicle_source_url";
    private const string ObjectDataColumn = "object_data";
    private const string KindNameColumn = "kind_name";
    private const string BrandNameColumn = "brand_name";
    private const string ModelNameColumn = "model_name";
    private const string LocationNameColumn = "location_name";
    private const string LocationKindColumn = "location_kind";
    private const string LocationCityColumn = "location_city";
    private const string PhotosColumn = "photos";
    private const string PhotoSourceColumn = "source";

    public RecentVehicle(DbDataReader reader)
    {
        Id = reader.GetString(reader.GetOrdinal(VehicleIdColumn));
        Price = reader.GetInt64(reader.GetOrdinal(VehiclePriceColumn));
        IsNds = reader.GetBoolean(reader.GetOrdinal(VehicleNdsColumn));
        BrandId = reader.GetGuid(reader.GetOrdinal(BrandIdColumn));
        CategoryId = reader.GetGuid(reader.GetOrdinal(KindIdColumn));
        ModelId = reader.GetGuid(reader.GetOrdinal(ModelIdColumn));
        RegionId = reader.GetGuid(reader.GetOrdinal(GeoIdColumn));
        Description = reader.GetString(reader.GetOrdinal(DescriptionColumn));
        SourceUrl = reader.GetString(reader.GetOrdinal(SourceUrlColumn));
        JsonDocument document = JsonDocument.Parse(
            reader.GetString(reader.GetOrdinal(ObjectDataColumn))
        );
        CategoryName = NamedStringFromJson(document, KindNameColumn);
        BrandName = NamedStringFromJson(document, BrandNameColumn);
        ModelName = NamedStringFromJson(document, ModelNameColumn);
        Region = NamedStringFromJson(document, LocationNameColumn);
        RegionKind = NamedStringFromJson(document, LocationKindColumn);
        City = NamedStringFromJson(document, LocationCityColumn);
        Photos = PhotosFromJson(document);
        document.Dispose();
    }

    private static string NamedStringFromJson(JsonDocument document, string name)
    {
        return document.RootElement.GetProperty(name).GetString()!;
    }

    private static IEnumerable<string> PhotosFromJson(JsonDocument document)
    {
        List<string> photos = [];
        foreach (
            JsonElement json in document.RootElement.GetProperty(PhotosColumn).EnumerateArray()
        )
            photos.Add(json.GetProperty(PhotoSourceColumn).GetString()!);
        return photos;
    }
}
