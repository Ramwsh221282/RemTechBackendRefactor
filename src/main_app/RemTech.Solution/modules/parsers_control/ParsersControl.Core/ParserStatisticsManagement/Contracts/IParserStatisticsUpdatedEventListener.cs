using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserStatisticsManagement.Contracts;

public interface IParserStatisticsUpdatedEventListener
{
    Task<Result<Unit>> React(ParserStatisticData data, CancellationToken ct = default);
}