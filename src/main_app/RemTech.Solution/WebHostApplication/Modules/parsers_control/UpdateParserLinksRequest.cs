namespace WebHostApplication.Modules.parsers_control;

public sealed record UpdateParserLinksRequest(IEnumerable<UpdateParserLinksRequestPayload> Links);
