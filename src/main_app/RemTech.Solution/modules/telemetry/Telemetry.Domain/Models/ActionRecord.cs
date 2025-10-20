using Telemetry.Domain.Models.ValueObjects;

namespace Telemetry.Domain.Models;

/// <summary>
/// Запись действия пользователя
/// </summary>
public class ActionRecord
{
    /// <summary>
    /// Идентификатор записи
    /// </summary>
    public required ActionId Id { get; init; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public required ActionInvokerId InvokerId { get; init; }

    /// <summary>
    /// Комментарии к действию.
    /// </summary>
    public required IReadOnlyList<ActionComment> Comments { get; init; }

    /// <summary>
    /// Статус операции (успешный или ошибка)
    /// </summary>
    public required ActionStatus Status { get; init; }

    /// <summary>
    /// Дата создания записи
    /// </summary>
    public required ActionDate OccuredAt { get; init; }

    /// <summary>
    /// Название действия
    /// </summary>
    public required ActionName Name { get; init; }
}
