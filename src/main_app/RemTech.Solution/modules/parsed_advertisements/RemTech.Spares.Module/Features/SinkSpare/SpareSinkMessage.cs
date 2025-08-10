namespace RemTech.Spares.Module.Features.SinkSpare;

internal sealed record SpareSinkMessage(ParserBody Parser, ParserLinkBody Link, SpareBody Spare);
