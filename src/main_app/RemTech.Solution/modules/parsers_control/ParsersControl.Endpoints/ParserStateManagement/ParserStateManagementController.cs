using Microsoft.AspNetCore.Mvc;
using ParsersControl.Endpoints.Common;
using ParsersControl.Presenters.ParserStateManagement.Common;
using ParsersControl.Presenters.ParserStateManagement.Disable;
using ParsersControl.Presenters.ParserStateManagement.Enable;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;

namespace ParsersControl.Endpoints.ParserStateManagement;

[ApiController]
[Route(Constants.StateManagementRoute)]
public sealed class ParserStateManagementController
{
    [HttpPatch("{id:guid}/disabled")]
    public async Task<Envelope> Disable(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] IGateway<DisableParserRequest, ParserStateChangeResponse> handler,
        CancellationToken ct
        )
    {
        DisableParserRequest gatewayRequest = new(id, ct);
        Result<ParserStateChangeResponse> result = await handler.Execute(gatewayRequest);
        return result.AsEnvelope();
    }

    [HttpPatch("{id:guid}/enabled")]
    public async Task<Envelope> Enable(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] IGateway<EnableParserGatewayRequest, ParserStateChangeResponse> handler,
        CancellationToken ct
        )
    {
        EnableParserGatewayRequest request = new(id, ct);
        Result<ParserStateChangeResponse> result = await handler.Execute(request);
        return result.AsEnvelope();
    }
}