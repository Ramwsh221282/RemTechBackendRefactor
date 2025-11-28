using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Presenters.ParserStateManagement.Common;
using ParsersControl.Presenters.ParserStateManagement.StartWaiting;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParsersControl.Features;

public sealed class MakeWaitingParserFeature(IServiceProvider sp)
{
    public async Task<Result<ParserStateChangeResponse>> Invoke(Guid id)
    {
        CancellationToken ct = CancellationToken.None;
        StartWaitingRequest request = new(id ,ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<StartWaitingRequest, ParserStateChangeResponse> service =
            scope.Resolve<IGateway<StartWaitingRequest, ParserStateChangeResponse>>();
        return await service.Execute(request);
    }
}