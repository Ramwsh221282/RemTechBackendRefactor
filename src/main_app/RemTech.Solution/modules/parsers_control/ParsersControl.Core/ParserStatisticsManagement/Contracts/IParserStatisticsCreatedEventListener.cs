using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserStatisticsManagement.Contracts;

public interface IParserStatisticsCreatedEventListener
{
    Task<Result<Unit>> React(ParserStatisticData data, CancellationToken ct = default);
}