using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParserLinkManagement.Feature;

public sealed class RenameParserLinkFeature(IServiceProvider sp)
{
    public async Task<Result<ParserLinkResponse>> Invoke(
        Guid linkId,
        string newName
    )
    {
        CancellationToken ct = CancellationToken.None;
        RenameParserLinkRequest request = new(linkId, newName, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<RenameParserLinkRequest, ParserLinkResponse> service =
            scope.Resolve<IGateway<RenameParserLinkRequest, ParserLinkResponse>>();
        return await service.Execute(request);
    }
}