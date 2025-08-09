using Drom.Parsing.Vehicles.Parsing.Models;

namespace Drom.Parsing.Vehicles.Parsing.Logging;

public sealed class DromCatalogueCarLogMessage
{
    private readonly string _id;
    private readonly string[] _photos;
    private readonly string _source;
    private readonly string _model;
    private readonly string _brand;
    private readonly string _kind;
    private readonly long _price;
    private readonly bool _isNds;
    private readonly CarCharacteristicsCollectionLogMessage _characteristicsLog;

    public DromCatalogueCarLogMessage(
        string id,
        string[] photos,
        string source,
        string model,
        string brand,
        string kind,
        long price,
        bool isNds,
        CarCharacteristicsCollection characteristics
    )
    {
        _id = id;
        _photos = photos;
        _source = source;
        _model = model;
        _brand = brand;
        _kind = kind;
        _price = price;
        _isNds = isNds;
        _characteristicsLog = characteristics.LogMessage();
    }

    public void Log(Serilog.ILogger logger)
    {
        logger.Information(
            """
            CAR INFORMATION:
            ID: {id},
            Photos Count: {pCount},
            Model: {model},
            Brand: {brand},
            Kind: {kind},
            Price: {price},
            Is nds: {isNds}.
            """,
            _id,
            _photos.Length,
            _model,
            _brand,
            _kind,
            _price,
            _isNds
        );
        _characteristicsLog.Log(logger);
    }
}
