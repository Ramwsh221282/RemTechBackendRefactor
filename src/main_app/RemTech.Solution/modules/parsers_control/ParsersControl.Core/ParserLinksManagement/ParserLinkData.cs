namespace ParsersControl.Core.ParserLinksManagement;

public record ParserLinkData(Guid Id, string Name, string Url, bool Ignored, Guid ParserId);