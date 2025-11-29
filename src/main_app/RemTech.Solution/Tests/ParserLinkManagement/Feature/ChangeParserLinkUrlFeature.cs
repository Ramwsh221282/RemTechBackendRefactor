using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Presenters.ParserLinkManagement.ChangeUrlParserLink;
using ParsersControl.Presenters.ParserLinkManagement.Common;
using RemTech.SharedKernel.Configuration;
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
        ChangeParserLinkRequest request = new(linkId, newUrl, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<ChangeParserLinkRequest, ParserLinkResponse> service =
            scope.Resolve<IGateway<ChangeParserLinkRequest, ParserLinkResponse>>();
        return await service.Execute(request);
    }
}