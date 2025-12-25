using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParsersControl.Features;

public sealed class SetFinishedFeature(IServiceProvider sp)
{
    public async Task<Result<ParserScheduleUpdateResponse>> Invoke(Guid id, DateTime finished)
    {
        CancellationToken ct = CancellationToken.None;
        SetFinishedDateRequest request = new(id, finished, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<SetFinishedDateRequest, ParserScheduleUpdateResponse> service =
            scope.Resolve<IGateway<SetFinishedDateRequest, ParserScheduleUpdateResponse>>();
        return await service.Execute(request);
    }
}