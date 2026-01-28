using System.Text.Json;
using RemTech.SharedKernel.Core.Handlers;

namespace Spares.Infrastructure.Queries.GetSpares;

/// <summary>
/// Запрос на получение запчастей.
/// </summary>
public sealed class GetSparesQuery : IQuery
{
	/// <summary>
	/// Идентификатор региона для фильтрации запчастей.
	/// </summary>
	public Guid? RegionId { get; private set; }

	/// <summary>
	/// Минимальная цена запчасти для фильтрации.
	/// </summary>
	public long? PriceMin { get; private set; }

	/// <summary>
	/// Максимальная цена запчасти для фильтрации.
	/// </summary>
	public long? PriceMax { get; private set; }

	/// <summary>
	/// Текстовый поиск по запчастям.
	/// </summary>
	public string? TextSearch { get; private set; }

	/// <summary>
	/// Номер страницы результатов.
	/// </summary>
	public int Page { get; private set; } = 1;

	/// <summary>
	/// Размер страницы результатов.
	/// </summary>
	public int PageSize { get; private set; } = 50;

	/// <summary>
	/// 	Режим сортировки результатов.
	/// </summary>
	public string OrderMode { get; private set; } = "NONE";

	/// <summary>
	/// OEM запчасти для фильтрации.
	/// </summary>
	public string? Oem { get; private set; }

	/// <summary>
	/// Тип запчасти для фильтрации.
	/// </summary>
	public string? Type { get; private set; }

	/// <summary>
	/// Фильтрует запчасти по типу.
	/// </summary>
	/// <param name="type"> Тип запчасти для фильтрации.</param>
	/// <returns> Текущий экземпляр запроса с примененным фильтром по типу.</returns>
	public GetSparesQuery ForType(string? type)
	{
		if (!string.IsNullOrWhiteSpace(Type))
			return this;
		if (string.IsNullOrWhiteSpace(type))
			return this;
		Type = type;
		return this;
	}

	/// <summary>
	/// Фильтрует запчасти по региону.
	/// </summary>
	/// <param name="regionId"> Идентификатор региона для фильтрации.</param>
	/// <returns> Текущий экземпляр запроса с примененным фильтром по региону.</returns>
	public GetSparesQuery ForRegion(Guid? regionId)
	{
		if (RegionId.HasValue)
			return this;
		if (regionId is null)
			return this;
		RegionId = regionId;
		return this;
	}

	/// <summary>
	/// Фильтрует запчасти по OEM.
	/// </summary>
	/// <param name="oem"> OEM запчасти для фильтрации.</param>
	/// <returns> Текущий экземпляр запроса с примененным фильтром по OEM.</returns>
	public GetSparesQuery ForOem(string? oem)
	{
		if (!string.IsNullOrWhiteSpace(Oem))
			return this;
		if (string.IsNullOrWhiteSpace(oem))
			return this;
		Oem = oem;
		return this;
	}

	/// <summary>
	/// Фильтрует запчасти по минимальной цене.
	/// </summary>
	/// <param name="price"> Минимальная цена запчасти для фильтрации.</param>
	/// <returns> Текущий экземпляр запроса с примененным фильтром по минимальной цене.</returns>
	public GetSparesQuery WithMinimalPrice(long? price)
	{
		if (PriceMin.HasValue)
			return this;
		if (price is null)
			return this;
		PriceMin = price;
		return this;
	}

	/// <summary>
	/// Фильтрует запчасти по максимальной цене.
	/// </summary>
	/// <param name="price"> Максимальная цена запчасти для фильтрации.</param>
	/// <returns> Текущий экземпляр запроса с примененным фильтром по максимальной цене.</returns>
	public GetSparesQuery WithMaximalPrice(long? price)
	{
		if (PriceMax.HasValue)
			return this;
		if (price is null)
			return this;
		PriceMax = price;
		return this;
	}

	/// <summary>
	/// Фильтрует запчасти по текстовому поиску.
	/// </summary>
	/// <param name="text"> Текст для фильтрации запчастей.</param>
	/// <returns> Текущий экземпляр запроса с примененным фильтром по текстовому поиску.</returns>
	public GetSparesQuery WithTextSearch(string? text)
	{
		if (!string.IsNullOrWhiteSpace(TextSearch))
			return this;
		if (string.IsNullOrWhiteSpace(text))
			return this;
		TextSearch = text;
		return this;
	}

	/// <summary>
	/// Устанавливает режим сортировки результатов.
	/// </summary>
	/// <param name="mode"> Режим сортировки результатов.</param>
	/// <returns> Текущий экземпляр запроса с примененным режимом сортировки.</returns>
	public GetSparesQuery WithOrderMode(string? mode)
	{
		if (string.IsNullOrWhiteSpace(mode))
			return this;
		OrderMode = mode;
		return this;
	}

	/// <summary>
	/// Устанавливает номер страницы результатов.
	/// </summary>
	/// <param name="page"> Номер страницы результатов.</param>
	/// <returns> Текущий экземпляр запроса с установленным номером страницы.</returns>
	public GetSparesQuery WithPage(int? page)
	{
		if (page is null)
			return this;
		if (page.Value <= 0)
			return this;
		Page = page.Value;
		return this;
	}

	/// <summary>
	/// Устанавливает размер страницы результатов.
	/// </summary>
	/// <param name="size"> Размер страницы результатов.</param>
	/// <returns> Текущий экземпляр запроса с установленным размером страницы.</returns>
	public GetSparesQuery WithPageSize(int? size)
	{
		if (size is null)
			return this;
		if (size.Value > 50)
			return this;
		PageSize = size.Value;
		return this;
	}

	/// <summary>
	/// Преобразует запрос в строковое представление JSON.
	/// </summary>
	/// <returns>Строковое представление запроса в формате JSON.</returns>
	public override string ToString() => JsonSerializer.Serialize(this);
}
