using DromVehiclesParser.Parsing.ParsingStages.Models;
using RemTech.SharedKernel.Infrastructure.Database;

namespace DromVehiclesParser.Parsing.ParsingStages.StageProcessStrategies;

public delegate Task ParsingStage(
    ParsingStageDependencies deps, 
    ParserWorkStage stage, 
    NpgSqlSession session, 
    CancellationToken ct);