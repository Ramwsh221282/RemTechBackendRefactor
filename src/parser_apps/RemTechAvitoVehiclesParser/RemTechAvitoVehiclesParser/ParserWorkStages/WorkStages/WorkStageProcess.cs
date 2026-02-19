using RemTech.SharedKernel.Infrastructure.Database;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Models;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages;

public delegate Task WorkStageProcess(
    WorkStageProcessDependencies deps, 
    ParserWorkStage stage, 
    NpgSqlSession session, 
    CancellationToken ct = default);

