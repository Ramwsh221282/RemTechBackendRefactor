using System.Data.Common;
using System.Text.Json;

namespace WebHostApplication.Queries.Responses;

/// <summary>
/// Ответ с информацией о разрешениях пользователя.
/// </summary>
public sealed class ActionRecordUserPermissionResponse
{
	private static int? _ordinal;

	/// <summary>
	/// Идентификатор разрешения.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// Название разрешения.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Описание разрешения.
	/// </summary>
	public required string Description { get; set; }

	/// <summary>
	/// Создает массив разрешений пользователя из DbDataReader.
	/// </summary>
	/// <param name="reader">Объект DbDataReader для чтения данных.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Массив разрешений пользователя или null, если данные отсутствуют.</returns>
	public static async Task<IReadOnlyList<ActionRecordUserPermissionResponse>?> ArrayFromDbReader(
		DbDataReader reader,
		CancellationToken ct
	)
	{
		_ordinal ??= reader.GetOrdinal("user_permissions");
		if (await reader.IsDBNullAsync(_ordinal.Value, ct))
		{
			return null;
		}

		return JsonSerializer.Deserialize<IReadOnlyList<ActionRecordUserPermissionResponse>>(
			reader.GetString(_ordinal.Value)
		);
	}
}
