using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserScheduleManagement.SetFinishedDate;

public sealed record SetFinishedDateRequest(
    Guid Id, 
    DateTime FinishedAt, 
    CancellationToken Ct) : IRequest;