using ParsersControl.Core.ParserScheduleManagement;
using ParsersControl.Core.ParserScheduleManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserScheduleManagement.NpgSql;

public static class NpgSqlParserScheduleManagementExtensions
{
    extension(ParserScheduleData data)
    {
        public async Task<Result<Unit>> ValidateUniquesness(
            IParserScheduleStorage storage, 
            CancellationToken ct = default)
        {
            ParserScheduleQueryArgs query = new(Id: data.Id);
            ParserSchedule? schedule = await storage.Fetch(query, ct);
            return schedule == null 
                ? Unit.Value 
                : Error.Conflict("У парсера уже есть конфигурация расписания");
        }
    }
}