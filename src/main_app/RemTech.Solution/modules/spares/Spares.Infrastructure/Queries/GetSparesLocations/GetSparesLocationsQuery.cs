using System.Text.Json;
using RemTech.SharedKernel.Core.Handlers;

namespace Spares.Infrastructure.Queries.GetSparesLocations;

// TODO: amount is not used. Use it in query handler as LIMIT for sql.

/// <summary>
/// Запрос на получение локаций запчастей.
/// </summary>
public sealed class GetSparesLocationsQuery : IQuery
{
	private GetSparesLocationsQuery() { }

	/// <summary>
	/// Текст для поиска запчастей.
	/// </summary>
	public string? TextSearch { get; private init; }

	/// <summary>
	/// Количество запчастей для получения.
	/// </summary>
	public int? Amount { get; private init; }

	/// <summary>
	/// Создаёт запрос на получение локаций запчастей.
	/// </summary>
	/// <returns>Созданный запрос на получение локаций запчастей.</returns>
	public static GetSparesLocationsQuery Create() => new();

	/// <summary>
	/// Добавляет текст для поиска запчастей.
	/// </summary>
	/// <param name="text">Текст для поиска запчастей.</param>
	/// <returns>Обновлённый запрос на получение локаций запчастей с добавленным текстом для поиска.</returns>
	public GetSparesLocationsQuery WithTextSearch(string? text) => Copy(this, textSearch: text);

	/// <summary>
	/// Добавляет количество запчастей для получения.
	/// </summary>
	/// <param name="amount">Количество запчастей для получения.</param>
	/// <returns>Обновлённый запрос на получение локаций запчастей с добавленным количеством запчастей для получения.</returns>
	public GetSparesLocationsQuery WithAmount(int? amount) => Copy(this, amount: amount);

	/// <summary>
	/// Преобразует запрос в строку.
	/// </summary>
	/// <returns>Строковое представление запроса в формате JSON.</returns>
	public override string ToString() => JsonSerializer.Serialize(this);

	private static GetSparesLocationsQuery Copy(
		GetSparesLocationsQuery origin,
		string? textSearch = null,
		int? amount = null
	) =>
		new()
		{
			TextSearch = string.IsNullOrWhiteSpace(textSearch) ? origin.TextSearch : textSearch,
			Amount = amount ?? origin.Amount,
		};
}
