using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserScheduleManagement.UpdateWaitDays;

public sealed record UpdateWaitDaysRequest(Guid Id, int WaitDays, CancellationToken Ct) : IRequest;