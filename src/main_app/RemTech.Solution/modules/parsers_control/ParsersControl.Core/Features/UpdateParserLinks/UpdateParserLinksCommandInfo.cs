namespace ParsersControl.Core.Features.UpdateParserLinks;

public sealed record UpdateParserLinksCommandInfo(
    Guid LinkId,
    bool? Activity = null,
    string? Name = null,
    string? Url = null
);
