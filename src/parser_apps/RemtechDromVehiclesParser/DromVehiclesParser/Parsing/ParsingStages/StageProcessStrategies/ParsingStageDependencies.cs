using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;
using ParsingSDK.RabbitMq;

namespace DromVehiclesParser.Parsing.ParsingStages.StageProcessStrategies;

public sealed record ParsingStageDependencies(
    BrowserManagerProvider Browsers,     
    Serilog.ILogger Logger,
    FinishParserProducer FinishProducer,
    AddContainedItemProducer AddContainedItemProducer,
    ParserStopState StopState
);