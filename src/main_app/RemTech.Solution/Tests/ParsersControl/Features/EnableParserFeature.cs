using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParsersControl.Features;

public sealed class EnableParserFeature(IServiceProvider sp)
{
    public async Task<Result<ParserStateChangeResponse>> Invoke(Guid id)
    {
        CancellationToken ct = CancellationToken.None;
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        EnableParserGatewayRequest request = new(id, ct);
        IGateway<EnableParserGatewayRequest, ParserStateChangeResponse> service =
            scope.Resolve<IGateway<EnableParserGatewayRequest, ParserStateChangeResponse>>();
        return await service.Execute(request);
    }
}