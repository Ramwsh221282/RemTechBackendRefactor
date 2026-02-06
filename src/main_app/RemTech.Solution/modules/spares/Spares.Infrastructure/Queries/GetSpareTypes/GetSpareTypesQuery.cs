using System.Text.Json;
using RemTech.SharedKernel.Core.Handlers;

namespace Spares.Infrastructure.Queries.GetSpareTypes;

/// <summary>
/// Запрос на получение типов запчастей.
/// </summary>
public sealed class GetSpareTypesQuery : IQuery
{
	private GetSpareTypesQuery() { }

	/// <summary>
	/// Текст для поиска типов запчастей.
	/// </summary>
	public string? TextSearch { get; private init; }

	/// <summary>
	/// Количество типов запчастей для получения.
	/// </summary>
	public int? Amount { get; private init; }

	/// <summary>
	/// Создаёт запрос на получение типов запчастей.
	/// </summary>
	/// <returns>Созданный запрос на получение типов запчастей.</returns>
	public static GetSpareTypesQuery Create()
	{
		return new();
	}

	/// <summary>
	/// Добавляет текст для поиска типов запчастей.
	/// </summary>
	/// <param name="text">Текст для поиска типов запчастей.</param>
	/// <returns>Обновлённый запрос на получение типов запчастей.</returns>
	public GetSpareTypesQuery WithTextSearch(string? text)
	{
		return Clone(this, textSearch: text);
	}

	/// <summary>
	/// Добавляет количество типов запчастей для получения.
	/// </summary>
	/// <param name="amount">Количество типов запчастей для получения.</param>
	/// <returns>Обновлённый запрос на получение типов запчастей.</returns>
	public GetSpareTypesQuery WithAmount(int? amount)
	{
		return Clone(this, amount: amount);
	}

	/// <summary>
	/// Преобразует запрос в строку.
	/// </summary>
	/// <returns>Строковое представление запроса на получение типов запчастей.</returns>
	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}

	private static GetSpareTypesQuery Clone(GetSpareTypesQuery origin, string? textSearch = null, int? amount = null)
	{
		return new()
		{
			TextSearch = string.IsNullOrWhiteSpace(textSearch) ? origin.TextSearch : textSearch,
			Amount = amount ?? origin.Amount,
		};
	}
}
