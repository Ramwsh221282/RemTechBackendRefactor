using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParsersControl.Features;

public sealed class UpdateParserProcessedFeature(IServiceProvider sp)
{
    public async Task<Result<ParserStatisticsUpdateResponse>> Invoke(Guid id, int processed)
    {
        CancellationToken ct = CancellationToken.None;
        UpdateParserProcessedRequest request = new(id, processed, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<UpdateParserProcessedRequest, ParserStatisticsUpdateResponse> service =
            scope.Resolve<IGateway<UpdateParserProcessedRequest, ParserStatisticsUpdateResponse>>();
        return await service.Execute(request);
    }
}