namespace Spares.Domain.Models;

public sealed record Spare(ContainedItemId Id, SpareDetails Details, SpareSource Source);
