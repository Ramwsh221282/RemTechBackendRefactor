namespace Spares.Domain.Features;

public sealed record AddSparesCreatorPayload(Guid CreatorId, string CreatorDomain, string CreatorType);