using System.Text.Json;

namespace RemTech.Spares.Module.Features.QuerySpare;

internal sealed class SpareJsonObjectReader(string objectData) : IDisposable
{
    private readonly JsonDocument _document = JsonDocument.Parse(objectData);

    public SpareQueryResult Read()
    {
        string id = _document.RootElement.GetProperty("id").GetString()!;
        string city = _document.RootElement.GetProperty("city").GetString()!;
        bool isNds = _document.RootElement.GetProperty("isNds").GetBoolean();
        string title = _document.RootElement.GetProperty("title").GetString()!;
        string region = _document.RootElement.GetProperty("region").GetString()!;
        Guid regionId = _document.RootElement.GetProperty("region_id").GetGuid();
        Guid cityId = _document.RootElement.GetProperty("city_id").GetGuid();
        string sourceUrl = _document.RootElement.GetProperty("sourceUrl").GetString()!;
        long priceValue = _document.RootElement.GetProperty("priceValue").GetInt64();
        string description = _document.RootElement.GetProperty("description").GetString()!;
        string regionKind = _document.RootElement.GetProperty("region_kind").GetString()!;
        List<string> photos = [];
        foreach (var photoElement in _document.RootElement.GetProperty("photos").EnumerateArray())
            photos.Add(photoElement.GetString()!);
        return new SpareQueryResult(
            id,
            city,
            cityId,
            isNds,
            title,
            photos,
            region,
            regionId,
            sourceUrl,
            priceValue,
            description,
            regionKind
        );
    }

    public void Dispose()
    {
        _document.Dispose();
    }
}
