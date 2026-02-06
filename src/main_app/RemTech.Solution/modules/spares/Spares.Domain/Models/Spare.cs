using Spares.Domain.Oems;
using Spares.Domain.Types;

namespace Spares.Domain.Models;

/// <summary>
/// Запчасть.
/// </summary>
/// <param name="Id">Идентификатор запчасти.</param>
/// <param name="OemId">Идентификатор OEM запчасти.</param>
/// <param name="TypeId">Идентификатор типа запчасти.</param>
/// <param name="Details">Детали запчасти.</param>
/// <param name="Source">Источник запчасти.</param>
public sealed record Spare(ContainedItemId Id, SpareOem Oem, SpareType Type, SpareDetails Details, SpareSource Source);
