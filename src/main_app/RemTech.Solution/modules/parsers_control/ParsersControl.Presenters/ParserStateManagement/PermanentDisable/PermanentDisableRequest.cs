using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.PermanentDisable;

public sealed record PermanentDisableRequest(Guid Id, CancellationToken Ct) : IRequest, IGatewayRequestWithParserFetchById;