using Microsoft.AspNetCore.Mvc;
using ParsersControl.Endpoints.Common;
using ParsersControl.Presenters.ParserScheduleManagement.Common;
using ParsersControl.Presenters.ParserScheduleManagement.UpdateWaitDays;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;

namespace ParsersControl.Endpoints.ParserScheduleManagement;

[ApiController]
[Route(Constants.ScheduleManagementRoute)]
public sealed class ParserScheduleManagementController
{
    [HttpPatch("{id:guid}")]
    public async Task<Envelope> ChangeWaitDays(
        [FromRoute(Name = "id")] Guid parserId,
        [FromQuery(Name = "day")] int day,
        [FromServices] IGateway<UpdateWaitDaysRequest, ParserScheduleUpdateResponse> handler,
        CancellationToken ct
        )
    {
        UpdateWaitDaysRequest gatewayRequest = new(parserId, day, ct);
        Result<ParserScheduleUpdateResponse> response = await handler.Execute(gatewayRequest);
        return response.AsEnvelope();
    }
}