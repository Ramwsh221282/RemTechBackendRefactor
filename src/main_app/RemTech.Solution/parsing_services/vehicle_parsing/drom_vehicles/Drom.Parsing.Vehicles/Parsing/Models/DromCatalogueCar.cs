using Drom.Parsing.Vehicles.Parsing.Logging;
using Parsing.RabbitMq.PublishVehicle;
using Parsing.RabbitMq.PublishVehicle.Extras;

namespace Drom.Parsing.Vehicles.Parsing.Models;

public sealed class DromCatalogueCar
{
    private readonly string[] _photos;
    private readonly string _id;
    private readonly string _source;
    private CarCharacteristicsCollection _characteristics;
    private string _model;
    private string _brand;
    private string _locationInfo;
    private string _kind;
    private long _price;
    private bool _isNds;
    private readonly SentencesCollection _sentences;

    public DromCatalogueCar(string id, string source, string[] photos)
    {
        _photos = photos;
        _id = id;
        _source = source;
        _model = string.Empty;
        _brand = string.Empty;
        _kind = string.Empty;
        _price = -1;
        _isNds = false;
        _locationInfo = string.Empty;
        _characteristics = new CarCharacteristicsCollection();
        _sentences = new SentencesCollection();
    }

    public string Id => _id;

    public void WithDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return;
        _sentences.Fill(description);
    }

    public void WithLocation(string locationInfo)
    {
        if (string.IsNullOrEmpty(locationInfo))
            return;
        _locationInfo = locationInfo;
    }

    public void WithModel(string model)
    {
        if (string.IsNullOrEmpty(model))
            return;
        if (!string.IsNullOrWhiteSpace(_model))
            throw new InvalidOperationException("Model is already present");
        _model = model;
    }

    public void WithBrand(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
            return;
        if (!string.IsNullOrWhiteSpace(_brand))
            throw new InvalidOperationException("Brand is already present");
        _brand = brand;
    }

    public void WithKind(string kind)
    {
        if (string.IsNullOrWhiteSpace(kind))
            return;
        if (!string.IsNullOrWhiteSpace(_kind))
            throw new InvalidOperationException("Kind is already present");
        _kind = kind;
    }

    public void WithCharacteristics(CarCharacteristicsCollection collection)
    {
        if (collection.Amount() == 0)
            return;
        _characteristics = collection;
    }

    public void WithCharacteristic(string name, string value)
    {
        if (name == "Б/у")
            return;
        _characteristics.With(name, value);
    }

    public void WithPrice(long value, bool isNds)
    {
        _price = value;
        _isNds = isNds;
    }

    public DromCatalogueCarLogMessage LogMessage() =>
        new(_id, _photos, _source, _model, _brand, _kind, _price, _isNds, _characteristics);

    public bool Valid()
    {
        return !string.IsNullOrWhiteSpace(_id)
            && !string.IsNullOrWhiteSpace(_source)
            && !string.IsNullOrWhiteSpace(_model)
            && !string.IsNullOrWhiteSpace(_brand)
            && !string.IsNullOrWhiteSpace(_kind)
            && _price > 0
            && _characteristics.Amount() > 0;
    }

    public DromCatalogueCarNavigation Navigation() => new(_source);

    public VehiclePublishMessage AsPublishMessage(
        string parserName,
        string parserType,
        string parserDomain,
        string linkName,
        string linkUrl
    )
    {
        ParserBody parserBody = new ParserBody(parserName, parserType, parserDomain);
        ParserLinkBody linkBody = new ParserLinkBody(
            parserName,
            parserType,
            parserDomain,
            linkName,
            linkUrl
        );
        VehicleBody vehicleBody = new VehicleBody(
            _id,
            _kind,
            _brand,
            _model,
            _price,
            _isNds,
            _locationInfo,
            _source,
            _sentences.FormText(),
            [],
            _photos.Select(s => new VehicleBodyPhoto(s))
        );
        return _characteristics.PrintTo(
            new VehiclePublishMessage(parserBody, linkBody, vehicleBody)
        );
    }
}
