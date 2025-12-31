using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParsersControl.Features;

public sealed class UpdateParserElapsedSecondsFeature(IServiceProvider sp)
{
    public async Task<Result<ParserStatisticsUpdateResponse>> Invoke(Guid id, long elapsedSeconds)
    {
        CancellationToken ct = CancellationToken.None;
        UpdateParserElapsedSecondsRequest request = new(id, elapsedSeconds, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<UpdateParserElapsedSecondsRequest, ParserStatisticsUpdateResponse> service =
            scope.Resolve<IGateway<UpdateParserElapsedSecondsRequest, ParserStatisticsUpdateResponse>>();
        return await service.Execute(request);
    }
}