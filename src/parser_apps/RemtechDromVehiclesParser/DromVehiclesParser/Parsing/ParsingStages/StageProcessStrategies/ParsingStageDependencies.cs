using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;
using ParsingSDK.RabbitMq;
using RemTech.SharedKernel.Infrastructure.Database;

namespace DromVehiclesParser.Parsing.ParsingStages.StageProcessStrategies;

public sealed record ParsingStageDependencies(
    BrowserManagerProvider Browsers,     
    Serilog.ILogger Logger,
    FinishParserProducer FinishProducer,
    AddContainedItemProducer AddContainedItemProducer,
    ParserStopState StopState
);