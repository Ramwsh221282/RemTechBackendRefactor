using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Presenters.ParserStateManagement.Common;
using ParsersControl.Presenters.ParserStateManagement.StartWorking;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParsersControl.Features;

public sealed class MakeWorkingParserFeature(IServiceProvider sp)
{
    public async Task<Result<ParserStateChangeResponse>> Invoke(Guid id)
    {
        CancellationToken ct = CancellationToken.None;
        StartWorkingRequest request = new(id, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<StartWorkingRequest, ParserStateChangeResponse> service =
            scope.Resolve<IGateway<StartWorkingRequest, ParserStateChangeResponse>>();
        return await service.Execute(request);
    }
}