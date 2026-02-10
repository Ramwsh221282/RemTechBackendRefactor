using AvitoFirewallBypass;
using ParsingSDK.Parsing;
using ParsingSDK.RabbitMq;
using ParsingSDK.TextProcessing;
using RemTech.SharedKernel.Infrastructure.Database;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages;

public sealed record WorkStageProcessDependencies(
    BrowserManagerProvider BrowserManagerProvider,
    AvitoBypassFactory Bypasses,
    TextTransformerBuilder TextTransformerBuilder,
    Serilog.ILogger Logger,
    NpgSqlConnectionFactory NpgSql,
    FinishParserProducer FinishProducer,
    AddContainedItemProducer AddContainedItemsProducer
);
