namespace WebHostApplication.Modules.parsers_control;

public sealed record AddLinksToParserRequest(IEnumerable<AddLinkToParserRequestBody> Links);