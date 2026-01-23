namespace WebHostApplication.Modules.parsers_control;

public sealed record UpdateParserLinksRequestPayload(
	Guid LinkId,
	bool? Activity = null,
	string? Name = null,
	string? Url = null
);
