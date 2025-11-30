using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStatisticsManagement.UpdateProcessed;

public sealed record UpdateParserProcessedRequest(Guid Id, int Processed, CancellationToken Ct) : IRequest;