using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParsersControl.Features;

public sealed class PermanentlyDisableParserFeature(IServiceProvider sp)
{
    public async Task<Result<ParserStateChangeResponse>> Invoke(Guid id)
    {
        CancellationToken ct = CancellationToken.None;
        PermanentDisableRequest request = new(id, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<PermanentDisableRequest, ParserStateChangeResponse> service =
            scope.Resolve<IGateway<PermanentDisableRequest, ParserStateChangeResponse>>();
        return await service.Execute(request);
    }
}