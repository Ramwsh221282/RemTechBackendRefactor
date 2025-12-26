using DromVehiclesParser.ResultsExporing.TextFileExporting;
using ParsingSDK.Parsing;
using RemTech.SharedKernel.Infrastructure.Database;

namespace DromVehiclesParser.Parsing.ParsingStages.StageProcessStrategies;

public sealed record ParsingStageDependencies(
    BrowserFactory Browsers, 
    NpgSqlConnectionFactory NpgSql, 
    Serilog.ILogger Logger,
    IExporter<TextFile> Exporter
);