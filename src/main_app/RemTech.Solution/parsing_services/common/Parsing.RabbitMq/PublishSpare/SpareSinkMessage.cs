using Parsing.RabbitMq.PublishVehicle;

namespace Parsing.RabbitMq.PublishSpare;

public sealed record SpareSinkMessage(ParserBody Parser, ParserLinkBody Link, SpareBody Spare);
