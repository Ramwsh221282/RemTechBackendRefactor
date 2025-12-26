using RemTech.Core.Shared.Result;
using Telemetry.Domain.Models;
using Telemetry.Domain.Models.ValueObjects;

namespace Telemetry.Domain.Ports.Storage;

/// <summary>
/// Порт для работы с хранилищем записей.
/// </summary>
public interface IActionRecordsStorage
{
    /// <summary>
    /// Получить список записей постранично.
    /// </summary>
    /// <param name="parameters">Параметры для пагинации, фильтрации и сортировки.</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список записей постранично.</returns>
    Task<ActionRecordsCollection> GetCollection(
        ReadActions parameters,
        CancellationToken ct = default
    );

    /// <summary>
    /// Добавить действие в хранилище
    /// </summary>
    Task Add(ActionRecord record, CancellationToken ct = default);

    /// <summary>
    /// Получить действие по его ИД (типизированный)
    /// </summary>
    Task<Status<ActionRecord>> GetById(ActionId recordId, CancellationToken ct = default);

    /// <summary>
    /// Добавить много записей bulk insert.
    /// </summary>
    /// <param name="records">Записи</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Добавленные записи</returns>
    Task<IEnumerable<ActionRecord>> AddRange(
        IEnumerable<ActionRecord> records,
        CancellationToken ct = default
    );

    Task<long> Count(CancellationToken ct = default);
}
