using Microsoft.AspNetCore.Mvc;
using ParsersControl.Endpoints.Common;
using ParsersControl.Presenters.ParserLinkManagement.AttachParserLink;
using ParsersControl.Presenters.ParserLinkManagement.ChangeUrlParserLink;
using ParsersControl.Presenters.ParserLinkManagement.Common;
using ParsersControl.Presenters.ParserLinkManagement.IgnoreParserLink;
using ParsersControl.Presenters.ParserLinkManagement.RenameParserLink;
using ParsersControl.Presenters.ParserLinkManagement.UnignoreParserLink;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;

namespace ParsersControl.Endpoints.ParserLinkManagement;

[ApiController]
[Route(Constants.LinksManagementRoute)]
public sealed class ParserLinkManagementController
{
    [HttpPost]
    public async Task<Envelope> AttachParserLink(
        [FromBody] ParserLinkAttachmentFrontendRequest request,
        [FromQuery(Name = "id")] Guid parserId,
        [FromServices] IGateway<AttachParserLinkRequest, ParserLinkResponse> gateway,
        CancellationToken ct
        )
    {
        AttachParserLinkRequest gatewayRequest = new(request.Name, request.Url, parserId, ct);
        Result<ParserLinkResponse> result = await gateway.Execute(gatewayRequest);
        return result.AsEnvelope();
    }

    [HttpPatch("{id:guid}/url")]
    public async Task<Envelope> ChangeUrl(
        [FromBody] ParserLinkChangeUrlFrontendRequest request,
        [FromRoute(Name = "id")] Guid linkId,
        [FromServices] IGateway<ChangeParserLinkUrlRequest, ParserLinkResponse> handler,
        CancellationToken ct
        )
    {
        ChangeParserLinkUrlRequest gatewayRequest = new(linkId, request.Url, ct);
        Result<ParserLinkResponse> result = await handler.Execute(gatewayRequest);
        return result.AsEnvelope();
    }

    [HttpPatch("{id:guid}/ignored")]
    public async Task<Envelope> Ignore(
        [FromRoute(Name = "id")] Guid linkId,
        [FromServices] IGateway<IgnoreParserLinkRequest, ParserLinkResponse> handler,
        CancellationToken ct)
    {
        IgnoreParserLinkRequest gatewayRequest = new(linkId, ct);
        Result<ParserLinkResponse> result = await handler.Execute(gatewayRequest);
        return result.AsEnvelope();
    }

    [HttpPatch("{id:guid}/unignored")]
    public async Task<Envelope> Unignore(
        [FromRoute(Name = "id")] Guid linkId,
        [FromServices] IGateway<UnignoreParserLinkRequest, ParserLinkResponse> handler,
        CancellationToken ct
        )
    {
        UnignoreParserLinkRequest gatewayRequest = new(linkId, ct);
        Result<ParserLinkResponse> result = await handler.Execute(gatewayRequest);
        return result.AsEnvelope();
    }

    [HttpPatch("{id:guid}/name")]
    public async Task<Envelope> ChangeName(
        [FromRoute(Name = "id")] Guid linkId,
        [FromQuery(Name = "name")] string name,
        [FromServices] IGateway<RenameParserLinkRequest, ParserLinkResponse> handler,
        CancellationToken ct = default
        )
    {
        RenameParserLinkRequest gatewayRequest = new(linkId, name, ct);
        Result<ParserLinkResponse> result = await handler.Execute(gatewayRequest);
        return result.AsEnvelope();
    }
}

public sealed record ParserLinkAttachmentFrontendRequest(string Name, string Url);
public sealed record ParserLinkChangeUrlFrontendRequest(string Url);