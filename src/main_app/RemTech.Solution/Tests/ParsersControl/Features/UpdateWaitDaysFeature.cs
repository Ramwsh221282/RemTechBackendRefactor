using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Presenters.ParserScheduleManagement.Common;
using ParsersControl.Presenters.ParserScheduleManagement.UpdateWaitDays;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParsersControl.Features;

public sealed class UpdateWaitDaysFeature(IServiceProvider sp)
{
    public async Task<Result<ParserScheduleUpdateResponse>> Invoke(Guid id, int waitDays)
    {
        CancellationToken ct = CancellationToken.None;
        UpdateWaitDaysRequest request = new(id, waitDays, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<UpdateWaitDaysRequest, ParserScheduleUpdateResponse> service =
            scope.Resolve<IGateway<UpdateWaitDaysRequest, ParserScheduleUpdateResponse>>();
        return await service.Execute(request);
    }
}