using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserScheduleManagement.Contracts;

public interface IParserScheduleUpdatedEventListener
{
    Task<Result<Unit>> React(ParserScheduleData data, CancellationToken ct = default);
}