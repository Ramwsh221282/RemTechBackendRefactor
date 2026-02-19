using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;
using ParsingSDK.RabbitMq;
using RemTech.SharedKernel.Infrastructure.Database;

namespace AvitoSparesParser.ParsingStages;

public sealed record ParserStageDependencies(    
    Serilog.ILogger Logger,
    AvitoBypassFactory Bypasses,
    BrowserManagerProvider BrowserProvider,
    TextTransformerBuilder TextTransformerBuilder,
    FinishParserProducer FinishProducer,
    AddContainedItemProducer AddContainedItem,
    ParserStopState StopState);
    
public delegate Task ParserStageProcess(
    ParserStageDependencies deps, 
    ParsingStage stage, 
    NpgSqlSession session, 
    CancellationToken ct);
