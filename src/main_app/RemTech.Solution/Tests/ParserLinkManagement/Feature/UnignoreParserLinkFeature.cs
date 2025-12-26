using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParserLinkManagement.Feature;

public sealed class UnignoreParserLinkFeature(IServiceProvider sp)
{
    public async Task<Result<ParserLinkResponse>> Invoke(Guid id)
    {
        CancellationToken ct = CancellationToken.None;
        UnignoreParserLinkRequest request = new(id, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<UnignoreParserLinkRequest, ParserLinkResponse> service =
            scope.Resolve<IGateway<UnignoreParserLinkRequest, ParserLinkResponse>>();
        return await service.Execute(request);
    }
}