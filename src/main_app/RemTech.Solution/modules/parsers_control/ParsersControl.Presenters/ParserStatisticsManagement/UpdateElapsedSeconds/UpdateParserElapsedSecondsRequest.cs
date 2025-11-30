using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStatisticsManagement.UpdateElapsedSeconds;

public sealed record UpdateParserElapsedSecondsRequest(
    Guid Id, 
    long ElapsedSeconds, 
    CancellationToken Ct) : IRequest;