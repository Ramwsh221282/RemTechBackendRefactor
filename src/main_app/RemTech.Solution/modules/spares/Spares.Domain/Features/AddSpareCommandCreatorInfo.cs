namespace Spares.Domain.Features;

public sealed record AddSpareCommandCreatorInfo(
    Guid CreatorId,
    string CreatorDomain,
    string CreatorType
);