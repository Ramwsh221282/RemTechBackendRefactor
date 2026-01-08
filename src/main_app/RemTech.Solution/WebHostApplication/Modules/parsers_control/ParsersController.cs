using System.Net;
using Microsoft.AspNetCore.Mvc;
using ParsersControl.Core.Features.AddParserLink;
using ParsersControl.Core.Features.EnableParser;
using ParsersControl.Core.Features.PermantlyDisableManyParsing;
using ParsersControl.Core.Features.PermantlyDisableParsing;
using ParsersControl.Core.Features.PermantlyStartManyParsing;
using ParsersControl.Core.Features.PermantlyStartParsing;
using ParsersControl.Core.Features.StartParserWork;
using ParsersControl.Core.Features.UpdateParserLinks;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.Queries.GetParsers;
using ParsersControl.WebApi.ResponseModels;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using WebHostApplication.ActionFilters.Attributes;

namespace WebHostApplication.Modules.parsers_control;

[ApiController]
[Route("api/parsers")]
public sealed class ParsersController : ControllerBase
{
    [VerifyToken]
    [ParserManagementPermission]
    [HttpPost("{id:guid}/start")]
    public async Task<Envelope> StartParser(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] ICommandHandler<StartParserCommand, SubscribedParser> handler,
        CancellationToken ct)
    {
        StartParserCommand command = new(Id: id);
        Result<SubscribedParser> result = await handler.Execute(command, ct);
        return result.AsTypedEnvelope(ParserResponse.Create);
    }

    [VerifyToken]
    [ParserManagementPermission]
    [HttpGet]
    public async Task<Envelope> GetParsers(
        [FromServices] IQueryHandler<GetParsersQuery, IEnumerable<ParserResponse>> handler,
        CancellationToken ct)
    {
        GetParsersQuery query = new();
        IEnumerable<ParserResponse> parsers = await handler.Handle(query, ct);
        return new Envelope((int)HttpStatusCode.OK, parsers, null);
    }
    
    [VerifyToken]
    [ParserManagementPermission]
    [HttpPatch("{id:guid}/permantly-start")]
    public async Task<Envelope> PermantlyStartParser(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] ICommandHandler<PermantlyStartParsingCommand, SubscribedParser> handler,
        CancellationToken ct)
    {
        PermantlyStartParsingCommand command = new(Id: id);
        Result<SubscribedParser> result = await handler.Execute(command, ct);
        return result.AsTypedEnvelope(ParserResponse.Create);
    }

    [VerifyToken]
    [ParserManagementPermission]
    [HttpPut("{id:guid}/links")]
    public async Task<Envelope> UpdateParserLinks(
        [FromRoute(Name = "id")] Guid id,
        [FromBody] UpdateParserLinksRequest request,
        [FromServices] ICommandHandler<UpdateParserLinksCommand, IEnumerable<SubscribedParserLink>> handler,
        CancellationToken ct
    )
    {
        IEnumerable<UpdateParserLinksCommandInfo> updateInfos = request.Links.Select(l => new UpdateParserLinksCommandInfo(l.LinkId, l.Activity, l.Name, l.Url));
        UpdateParserLinksCommand command = new(id, updateInfos);
        Result<IEnumerable<SubscribedParserLink>> result = await handler.Execute(command, ct);
        return result.AsTypedEnvelope(r => r.Select(ParserLinkResponse.Create));
    }

    [VerifyToken]
    [ParserManagementPermission]
    [HttpPatch("permantly-start")]
    public async Task<Envelope> PermantlyStartManyParsers(
        [FromQuery(Name = "ids")] IEnumerable<Guid> ids,
        [FromServices] ICommandHandler<PermantlyStartManyParsingCommand, IEnumerable<SubscribedParser>> handler,
        CancellationToken ct)
    {
        PermantlyStartManyParsingCommand command = new(Identifiers: ids);
        Result<IEnumerable<SubscribedParser>> result = await handler.Execute(command, ct);
        return result.AsTypedEnvelope(r => r.Select(ParserResponse.Create));
    }
    
    [VerifyToken]
    [ParserManagementPermission]
    [HttpPatch("permantly-disable")]
    public async Task<Envelope> PermantlyDisableManyParsers(
        [FromQuery(Name = "ids")] IEnumerable<Guid> ids,
        [FromServices] ICommandHandler<PermantlyDisableManyParsingCommand, IEnumerable<SubscribedParser>> handler,
        CancellationToken ct)
    {
        PermantlyDisableManyParsingCommand command = new(Identifiers: ids);
        Result<IEnumerable<SubscribedParser>> result = await handler.Execute(command, ct);
        return result.AsTypedEnvelope(r => r.Select(ParserResponse.Create));
    }
    
    [VerifyToken]
    [ParserManagementPermission]
    [HttpPatch("{id:guid}/permantly-disable")]
    public async Task<Envelope> PermantlyDisableParser(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] ICommandHandler<PermantlyDisableParsingCommand, SubscribedParser> handler,
        CancellationToken ct)
    {
        PermantlyDisableParsingCommand command = new(Id: id);
        Result<SubscribedParser> result = await handler.Execute(command, ct);
        return result.AsTypedEnvelope(ParserResponse.Create);
    }
    
    [VerifyToken]
    [ParserManagementPermission]
    [HttpPatch("{id:guid}/enable")]
    public async Task<Envelope> ChangeParserActivity(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] ICommandHandler<EnableParserCommand, SubscribedParser> handler,
        CancellationToken ct)
    {
        EnableParserCommand command = new(Id: id);
        Result<SubscribedParser> result = await handler.Execute(command, ct);
        return result.AsTypedEnvelope(ParserResponse.Create);
    }
    
    [VerifyToken]
    [ParserManagementPermission]
    [HttpPost("{id:guid}/links")]
    public async Task<Envelope> AddLinksToParser(
        [FromRoute (Name = "id")] Guid id,
        [FromBody] AddLinksToParserRequest request,
        [FromServices] ICommandHandler<AddParserLinkCommand, IEnumerable<SubscribedParserLink>> handler,
        CancellationToken ct
    )
    {
        AddParserLinkCommand command = new(id, request.Links.Select(l => new AddParserLinkCommandArg(l.Url, l.Name)));
        Result<IEnumerable<SubscribedParserLink>> result = await handler.Execute(command, ct);
        return result.AsTypedEnvelope(r => r.Select(ParserLinkResponse.Create));
    }
}