using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Presenters.ParserStateManagement.Common;
using ParsersControl.Presenters.ParserStateManagement.Disable;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParsersControl.Features;

public sealed class DisableParserFeature(IServiceProvider sp)
{
    public async Task<Result<ParserStateChangeResponse>> Invoke(Guid id)
    {
        CancellationToken ct = CancellationToken.None;
        DisableParserRequest request = new(id, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<DisableParserRequest, ParserStateChangeResponse> service =
            scope.Resolve<IGateway<DisableParserRequest, ParserStateChangeResponse>>();
        return await service.Execute(request);
    }
}