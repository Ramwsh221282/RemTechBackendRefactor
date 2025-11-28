using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.StartWaiting;

public sealed record StartWaitingRequest(Guid Id, CancellationToken Ct) : IRequest, IGatewayRequestWithParserFetchById;