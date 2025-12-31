using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParserLinkManagement.Feature;

public sealed class IgnoreParserLinkFeature(IServiceProvider sp)
{
    public async Task<Result<ParserLinkResponse>> Invoke(
        Guid linkId
    )
    {
        CancellationToken ct = CancellationToken.None;
        IgnoreParserLinkRequest request = new(linkId, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<IgnoreParserLinkRequest, ParserLinkResponse> service =
            scope.Resolve<IGateway<IgnoreParserLinkRequest, ParserLinkResponse>>();
        return await service.Execute(request);
    }
}