using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParserLinkManagement.Feature;

public sealed class AttachParserLinkFeature(IServiceProvider sp)
{
    public async Task<Result<ParserLinkResponse>> Invoke(
        Guid parserId,
        string name,
        string url
        )
    {
        CancellationToken ct = CancellationToken.None;
        AttachParserLinkRequest request = new(name, url, parserId, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<AttachParserLinkRequest, ParserLinkResponse> service =
            scope.Resolve<IGateway<AttachParserLinkRequest, ParserLinkResponse>>();
        return await service.Execute(request);
    }
}