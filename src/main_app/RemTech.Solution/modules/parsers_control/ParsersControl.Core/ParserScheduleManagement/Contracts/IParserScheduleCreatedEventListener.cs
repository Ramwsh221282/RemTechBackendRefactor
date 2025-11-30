using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserScheduleManagement.Contracts;

public interface IParserScheduleCreatedEventListener
{
    Task<Result<Unit>> React(ParserScheduleData data, CancellationToken ct = default);
}