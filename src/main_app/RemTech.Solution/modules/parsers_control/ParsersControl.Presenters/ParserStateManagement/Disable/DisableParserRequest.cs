using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.Disable;

public sealed record DisableParserRequest(Guid Id, CancellationToken Ct) : IRequest, IGatewayRequestWithParserFetchById;