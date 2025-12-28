using ParsingSDK.Parsing;
using ParsingSDK.RabbitMq;
using RemTech.SharedKernel.Infrastructure.Database;

namespace DromVehiclesParser.Parsing.ParsingStages.StageProcessStrategies;

public sealed record ParsingStageDependencies(
    BrowserFactory Browsers, 
    NpgSqlConnectionFactory NpgSql, 
    Serilog.ILogger Logger,
    FinishParserProducer FinishProducer,
    AddContainedItemProducer AddContainedItemProducer
);