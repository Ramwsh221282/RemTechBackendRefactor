using System.Text.Json;

namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

/// <summary>
/// Параметры запроса для получения транспортных средств.
/// </summary>
public sealed class GetVehiclesQueryParameters
{
	/// <summary>
	/// Идентификатор бренда транспортного средства.
	/// </summary>
	public Guid? BrandId { get; private set; }

	/// <summary>
	/// Идентификатор категории транспортного средства.
	/// </summary>
	public Guid? CategoryId { get; private set; }

	/// <summary>
	/// Идентификатор региона транспортного средства.
	/// </summary>
	public Guid? RegionId { get; private set; }

	/// <summary>
	/// Идентификатор модели транспортного средства.
	/// </summary>
	public Guid? ModelId { get; private set; }

	/// <summary>
	/// Флаг НДС.
	/// </summary>
	public bool? IsNds { get; private set; }

	/// <summary>
	/// Минимальная цена транспортного средства.
	/// </summary>
	public long? MinimalPrice { get; private set; }

	/// <summary>
	/// Максимальная цена транспортного средства.
	/// </summary>
	public long? MaximalPrice { get; private set; }

	/// <summary>
	/// Поля для сортировки транспортных средств.
	/// </summary>
	public IEnumerable<string>? SortFields { get; private set; }

	/// <summary>
	/// Тип сортировки транспортных средств.
	/// </summary>
	public string Sort { get; private set; } = "NONE";

	/// <summary>
	/// Номер страницы результатов.
	/// </summary>
	public int Page { get; private set; } = 1;

	/// <summary>
	/// Размер страницы результатов.
	/// </summary>
	public int PageSize { get; private set; } = 50;

	/// <summary>
	/// Текстовый поиск транспортных средств.
	/// </summary>
	public string? TextSearch { get; private set; }

	/// <summary>
	/// Характеристики транспортных средств для фильтрации.
	/// </summary>
	public Dictionary<Guid, string>? Characteristics { get; private set; }

	/// <summary>
	/// Фильтр по бренду.
	/// </summary>
	/// <param name="brandId">Идентификатор бренда транспортного средства.</param>
	/// <returns>Объект с обновленными параметрами запроса.</returns>
	public GetVehiclesQueryParameters ForBrand(Guid? brandId)
	{
		if (BrandId is not null)
		{
			return this;
		}

		if (brandId is null)
		{
			return this;
		}

		BrandId = brandId;
		return this;
	}

	/// <summary>
	/// Фильтр по категории.
	/// </summary>
	/// <param name="categoryId">Идентификатор категории транспортного средства.</param>
	/// <returns>Объект с обновленными параметрами запроса.</returns>
	public GetVehiclesQueryParameters ForCategory(Guid? categoryId)
	{
		if (CategoryId is not null)
		{
			return this;
		}
		if (categoryId is null)
		{
			return this;
		}

		CategoryId = categoryId;
		return this;
	}

	/// <summary>
	/// Фильтр по характеристикам.
	/// </summary>
	/// <typeparam name="T">Тип элементов источника характеристик.</typeparam>
	/// <param name="source">Источник характеристик.</param>
	/// <param name="converter">Функция преобразования элемента источника в пару идентификатор-название.</param>
	/// <returns>Объект с обновленными параметрами запроса.</returns>
	public GetVehiclesQueryParameters ForCharacteristics<T>(
		IEnumerable<T>? source,
		Func<T, (Guid Id, string Name)> converter
	)
	{
		if (Characteristics is not null)
		{
			return this;
		}
		if (source is null)
		{
			return this;
		}

		Dictionary<Guid, string> characteristics = [];

		foreach (T entry in source)
		{
			(Guid ctxId, string ctxName) = converter(entry);
			characteristics.TryAdd(ctxId, ctxName);
		}

		Characteristics = characteristics;
		return this;
	}

	/// <summary>
	/// Фильтр по текстовому поиску.
	/// </summary>
	/// <param name="textSearch">Текст для поиска транспортных средств.</param>
	/// <returns>Объект с обновленными параметрами запроса.</returns>
	public GetVehiclesQueryParameters ForTextSearch(string? textSearch)
	{
		if (!string.IsNullOrWhiteSpace(TextSearch))
		{
			return this;
		}

		if (string.IsNullOrEmpty(textSearch))
		{
			return this;
		}

		TextSearch = textSearch.Trim();
		return this;
	}

	/// <summary>
	/// Фильтр по региону.
	/// </summary>
	/// <param name="regionId">Идентификатор региона транспортного средства.</param>
	/// <returns>Объект с обновленными параметрами запроса.</returns>
	public GetVehiclesQueryParameters ForRegion(Guid? regionId)
	{
		if (RegionId is not null)
		{
			return this;
		}
		if (regionId is null)
		{
			return this;
		}
		RegionId = regionId;
		return this;
	}

	/// <summary>
	/// Фильтр по модели.
	/// </summary>
	/// <param name="modelId">Идентификатор модели транспортного средства.</param>
	/// <returns>Объект с обновленными параметрами запроса.</returns>
	public GetVehiclesQueryParameters ForModel(Guid? modelId)
	{
		if (ModelId is not null)
		{
			return this;
		}

		if (modelId is null)
		{
			return this;
		}

		ModelId = modelId;
		return this;
	}

	/// <summary>
	/// Фильтр по НДС.
	/// </summary>
	/// <param name="isNds">Флаг, указывающий, применяется ли НДС.</param>
	/// <returns>Объект с обновленными параметрами запроса.</returns>
	public GetVehiclesQueryParameters ForNds(bool? isNds)
	{
		if (IsNds is not null)
		{
			return this;
		}

		if (isNds is null)
		{
			return this;
		}
		IsNds = isNds;
		return this;
	}

	/// <summary>
	/// Фильтр по минимальной цене.
	/// </summary>
	/// <param name="minimalPrice">Минимальная цена транспортного средства.</param>
	/// <returns>Объект с обновленными параметрами запроса.</returns>
	public GetVehiclesQueryParameters ForMinimalPrice(long? minimalPrice)
	{
		if (MinimalPrice is not null)
		{
			return this;
		}

		if (minimalPrice is null)
		{
			return this;
		}
		MinimalPrice = minimalPrice;
		return this;
	}

	/// <summary>
	/// Фильтр по максимальной цене.
	/// </summary>
	/// <param name="maximalPrice">Максимальная цена транспортного средства.</param>
	/// <returns>Объект с обновленными параметрами запроса.</returns>
	public GetVehiclesQueryParameters ForMaximalPrice(long? maximalPrice)
	{
		if (MaximalPrice is not null)
		{
			return this;
		}
		if (maximalPrice is null)
		{
			return this;
		}
		MaximalPrice = maximalPrice;
		return this;
	}

	/// <summary>
	/// Фильтр по типу сортировки.
	/// </summary>
	/// <param name="sort">Тип сортировки транспортных средств.</param>
	/// <returns>Объект с обновленными параметрами запроса.</returns>
	public GetVehiclesQueryParameters ForSort(string? sort)
	{
		if (sort is null)
		{
			return this;
		}
		Sort = sort;
		return this;
	}

	/// <summary>
	/// Фильтр по полям сортировки.
	/// </summary>
	/// <param name="sortFields">Поля сортировки транспортных средств.</param>
	/// <returns>Объект с обновленными параметрами запроса.</returns>
	public GetVehiclesQueryParameters ForSortFields(IEnumerable<string>? sortFields)
	{
		if (sortFields is null)
		{
			return this;
		}
		SortFields = sortFields;
		return this;
	}

	/// <summary>
	/// Фильтр по номеру страницы.
	/// </summary>
	/// <param name="page">Номер страницы транспортных средств.</param>
	/// <returns>Объект с обновленными параметрами запроса.</returns>
	public GetVehiclesQueryParameters ForPage(int page)
	{
		if (page <= 0)
		{
			return this;
		}
		Page = page;
		return this;
	}

	/// <summary>
	/// Фильтр по размеру страницы.
	/// </summary>
	/// <param name="pageSize">Размер страницы транспортных средств.</param>
	/// <returns>Объект с обновленными параметрами запроса.</returns>
	public GetVehiclesQueryParameters ForPageSize(int pageSize)
	{
		if (pageSize <= 50)
		{
			return this;
		}
		PageSize = pageSize;
		return this;
	}

	/// <summary>
	/// Преобразует параметры запроса в строковое представление.
	/// </summary>
	/// <returns>Строковое представление параметров запроса.</returns>
	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}
}
