using ParsersControl.Core.ParserWorkStateManagement;
using ParsersControl.Core.ParserWorkStateManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserWorkStateManagement.Common;

public static class NpgSqlParserWorkStateExtensions
{
    extension(IParserWorkStatesStorage storage)
    {
        public async Task<Result<Unit>> ValidateUniquesness(ParserWorkTurnerState state, CancellationToken ct = default)
        {
            ParserWorkTurnerQueryArgs args = new(state.Id);
            ParserWorkTurner? workTurner = await storage.Fetch(args, ct);
            return workTurner == null ? Unit.Value : Error.Conflict("У парсера уже есть конфигурация состояния работы");
        }
    }
}