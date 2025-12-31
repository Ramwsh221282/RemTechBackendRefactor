namespace Spares.Domain.Models;

public sealed record Spare(
    SpareId Id,
    ContainedItemId ContainedItemId,
    SpareDetails Details,
    SpareSource Source
);