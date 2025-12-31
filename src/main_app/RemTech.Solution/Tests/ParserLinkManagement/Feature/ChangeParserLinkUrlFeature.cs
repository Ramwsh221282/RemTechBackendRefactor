using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParserLinkManagement.Feature;

public sealed class ChangeParserLinkUrlFeature(IServiceProvider sp)
{
    public async Task<Result<ParserLinkResponse>> Invoke(
        Guid linkId,
        string newUrl
    )
    {
        CancellationToken ct = CancellationToken.None;
        ChangeParserLinkUrlRequest urlRequest = new(linkId, newUrl, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<ChangeParserLinkUrlRequest, ParserLinkResponse> service =
            scope.Resolve<IGateway<ChangeParserLinkUrlRequest, ParserLinkResponse>>();
        return await service.Execute(urlRequest);
    }
}