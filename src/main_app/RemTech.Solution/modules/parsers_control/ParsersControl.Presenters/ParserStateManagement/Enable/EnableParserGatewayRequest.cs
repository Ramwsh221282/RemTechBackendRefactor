using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.Enable;

public sealed record EnableParserGatewayRequest(Guid Id, CancellationToken Ct) : IRequest, IGatewayRequestWithParserFetchById;