namespace Spares.Domain.Models;

/// <summary>
/// Запчасть.
/// </summary>
/// <param name="Id">Идентификатор запчасти.</param>
/// <param name="Details">Детали запчасти.</param>
/// <param name="Source">Источник запчасти.</param>
public sealed record Spare(ContainedItemId Id, SpareDetails Details, SpareSource Source);
