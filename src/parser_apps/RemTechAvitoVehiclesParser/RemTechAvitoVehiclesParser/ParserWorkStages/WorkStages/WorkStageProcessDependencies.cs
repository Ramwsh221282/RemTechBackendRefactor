using AvitoFirewallBypass;
using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;
using ParsingSDK.RabbitMq;
using ParsingSDK.TextProcessing;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages;

public sealed record WorkStageProcessDependencies(
    BrowserManagerProvider BrowserManagerProvider,
    AvitoBypassFactory Bypasses,
    TextTransformerBuilder TextTransformerBuilder,
    Serilog.ILogger Logger,    
    FinishParserProducer FinishProducer,
    AddContainedItemProducer AddContainedItemsProducer,
    ParserStopState StopState
);
