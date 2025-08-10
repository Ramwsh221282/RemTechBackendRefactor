using Parsing.RabbitMq.PublishSpare;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.Parsing;

public sealed class AvitoSpare
{
    private readonly string _id;
    private readonly string _sourceUrl;
    private readonly string _title;
    private string _oem;
    private readonly string _relatedBrand;
    private readonly string _geo;
    private readonly IEnumerable<string> _photos;
    private readonly long _price;
    private readonly bool _isNds;
    private readonly List<string> _descriptionDetails = [];

    public AvitoSpare(
        string id,
        string sourceUrl,
        string title,
        string oem,
        string relatedBrand,
        string geo,
        IEnumerable<string> photos,
        long price,
        bool isNds
    )
    {
        _id = $"AS{id}";
        _sourceUrl = $"https://www.avito.ru{sourceUrl}";
        _title = title;
        _oem = oem;
        _photos = photos;
        _price = price;
        _isNds = isNds;
        _relatedBrand = relatedBrand;
        _geo = geo;
    }

    public async Task Navigate(IPage page)
    {
        await new PageNavigating(page, _sourceUrl).Do();
    }

    public void CorrectOem(string oem)
    {
        _oem = oem;
    }

    public void AddDescriptionDetail(string detail)
    {
        _descriptionDetails.Add(detail);
    }

    public SpareBody AsSpareBody()
    {
        return new SpareBody(
            _id,
            MakeRawDescription(),
            _title,
            _sourceUrl,
            _price,
            _isNds,
            _geo,
            _photos
        );
    }

    private string MakeRawDescription()
    {
        return string.Join(
            ' ',
            _title,
            _oem,
            $"{_price} {(_isNds ? "с НДС" : "без НДС")}",
            _oem,
            _relatedBrand,
            _geo,
            _descriptionDetails
        );
    }
}
