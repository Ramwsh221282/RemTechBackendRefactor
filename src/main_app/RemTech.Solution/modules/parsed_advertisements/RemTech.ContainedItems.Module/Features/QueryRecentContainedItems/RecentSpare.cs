using System.Data.Common;
using System.Text.Json;

namespace RemTech.ContainedItems.Module.Features.QueryRecentContainedItems;

internal sealed record RecentSpare : SomeRecentItem
{
    private const string ObjectColumn = "object";
    private const string IdColumn = "id";
    private const string CityColumn = "city";
    private const string IsNdsColumn = "isNds";
    private const string RegionColumn = "region";
    private const string SourceUrlColumn = "sourceUrl";
    private const string PriceValueColumn = "priceValue";
    private const string DescriptionColumn = "description";
    private const string RegionKindColumn = "region_kind";
    private const string PhotosColumn = "photos";

    public RecentSpare(DbDataReader reader)
    {
        string @object = reader.GetString(reader.GetOrdinal(ObjectColumn));
        JsonDocument document = JsonDocument.Parse(@object);
        Id = document.RootElement.GetProperty(IdColumn).GetString()!;
        City = document.RootElement.GetProperty(CityColumn).GetString()!;
        IsNds = document.RootElement.GetProperty(IsNdsColumn).GetBoolean();
        Region = document.RootElement.GetProperty(RegionColumn).GetString()!;
        SourceUrl = document.RootElement.GetProperty(SourceUrlColumn).GetString()!;
        Price = document.RootElement.GetProperty(PriceValueColumn).GetInt64();
        Description = document.RootElement.GetProperty(DescriptionColumn).GetString()!;
        RegionKind = document.RootElement.GetProperty(RegionKindColumn).GetString()!;
        List<string> photos = [];
        photos.AddRange(
            document
                .RootElement.GetProperty(PhotosColumn)
                .EnumerateArray()
                .Select(photoElement => photoElement.GetString()!)
        );
        Photos = photos;
        document.Dispose();
    }
}
