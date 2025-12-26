using Microsoft.AspNetCore.Mvc;
using ParsersControl.Core.Features.AddParserLink;
using ParsersControl.Core.Features.StartParserWork;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.WebApi.ResponseModels;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;

namespace ParsersControl.WebApi.Controllers;

[ApiController]
[Route("api/parsers")]
public sealed class ParsersController : ControllerBase
{
    [HttpPost("{id:guid}/start")]
    public async Task<Envelope> StartParser(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] ICommandHandler<StartParserCommand, SubscribedParser> handler,
        CancellationToken ct)
    {
        StartParserCommand command = new(Id: id);
        Result<SubscribedParser> result = await handler.Execute(command, ct);
        return result.AsTypedEnvelope(ParserResponseModel.ConvertFrom);
    }

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
        return result.AsTypedEnvelope(ParserLinkResponseModel.ConvertFrom);        
    }
}

public sealed record AddLinksToParserRequest(IEnumerable<AddLinkToParserRequestBody> Links);
public sealed record AddLinkToParserRequestBody(string Name, string Url);

public sealed class ParserLinkResponseModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Url { get; init; }
    public required int ParsedCount { get; init; }
    public required long WorkTime { get; init; }
    public required bool IsActive { get; init; }
    public required int Hours { get; init; }
    public required int Minutes { get; init; }
    public required int Seconds { get; init; }

    public static ParserLinkResponseModel ConvertFrom(SubscribedParserLink link)
    {
        return new ParserLinkResponseModel()
        {
            Id = link.Id.Value,
            Name = link.UrlInfo.Name,
            Url = link.UrlInfo.Url,
            ParsedCount = link.Statistics.ParsedCount.Value,
            WorkTime = link.Statistics.WorkTime.TotalElapsedSeconds,
            IsActive = link.Active,
            Hours = link.Statistics.WorkTime.Hours,
            Minutes = link.Statistics.WorkTime.Minutes,
            Seconds = link.Statistics.WorkTime.Seconds
        };
    }
    
    public static IEnumerable<ParserLinkResponseModel> ConvertFrom(IEnumerable<SubscribedParserLink> links)
    {
        return links.Select(ConvertFrom).ToArray();
    }
}