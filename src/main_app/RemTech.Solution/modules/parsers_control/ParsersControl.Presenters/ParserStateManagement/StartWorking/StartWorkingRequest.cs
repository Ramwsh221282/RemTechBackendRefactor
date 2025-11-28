using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.StartWorking;

public sealed record StartWorkingRequest(Guid Id, CancellationToken Ct) : IRequest, IGatewayRequestWithParserFetchById;