using ParsersControl.Core.ParserStatisticsManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserStatisticsManagement.Defaults;

public sealed class NoneParserStatisticsUpdatedEventListener : IParserStatisticsUpdatedEventListener
{
    public Task<Result<Unit>> React(ParserStatisticData data, CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success(Unit.Value));
    }
}