using System.Text.RegularExpressions;
using RemTech.Core.Shared.Primitives;

namespace Avito.Vehicles.Service.VehiclesParsing.CatalogueItems;

public sealed class CatalogueItem
{
    private readonly NotEmptyString[] _photos;
    private readonly NotEmptyString _url;
    private readonly NotEmptyString _id;

    public CatalogueItem(NotEmptyString url, NotEmptyString[] photos)
    {
        _url = url;
        _photos = photos;
        _id = new NotEmptyString(string.Empty);
    }

    public CatalogueItem(NotEmptyString url, NotEmptyString id, NotEmptyString[] photos)
    {
        _url = url;
        _photos = photos;
        _id = new NotEmptyString(id);
    }

    public CatalogueItem(CatalogueItem item)
    {
        _url = item._url;
        _photos = item._photos;
        _id = item._id;
    }

    public CatalogueItem(CatalogueItem item, NotEmptyString id)
        : this(item) => _url = id;

    public CatalogueItem(CatalogueItem item, NotEmptyString[] photos)
        : this(item) => _photos = photos;

    public CatalogueItem()
    {
        _url = new NotEmptyString(string.Empty);
        _photos = [];
        _id = new NotEmptyString(string.Empty);
    }

    public NotEmptyString[] ReadPhotos() => _photos;

    public NotEmptyString ReadUrl() => _url;

    public NotEmptyString ReadId() => _id;

    public CatalogueItem Identified()
    {
        if (!_url)
            return this;
        string regExp = string.Intern(@"(\d+)[?]");
        Match match = Regex.Match(_url, regExp);
        if (match.Success == false)
            return this;
        NotEmptyString id = new NotEmptyString(match.Groups[1].Value);
        return new CatalogueItem(_url, id, _photos);
    }
}
