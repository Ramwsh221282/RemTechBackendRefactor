using System.Text.Json;
using System.Text.Json.Serialization;
using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Categories.Queries.GetCategories;

/// <summary>
/// Запрос получения категорий транспортных средств.
/// </summary>
public class GetCategoriesQuery : IQuery
{
	[JsonIgnore]
	private Dictionary<string, string>? _includedInformationKeys_cached;

	/// <summary>
	/// Идентификатор бренда транспортного средства.
	/// </summary>
	public Guid? BrandId { get; private init; }

	/// <summary>
	/// Название бренда транспортного средства.
	/// </summary>
	public string? BrandName { get; private init; }

	/// <summary>
	/// Идентификатор модели транспортного средства.
	/// </summary>
	public Guid? ModelId { get; private init; }

	/// <summary>
	/// Название модели транспортного средства.
	/// </summary>
	public string? ModelName { get; private init; }

	/// <summary>
	/// Идентификатор категории транспортного средства.
	/// </summary>
	public Guid? Id { get; private init; }

	/// <summary>
	/// Название категории транспортного средства.
	/// </summary>
	public string? Name { get; private init; }

	/// <summary>
	/// Включаемая информация в ответ.
	/// </summary>
	public IEnumerable<string>? IncludedInformation { get; private set; }

	/// <summary>
	/// Поля для сортировки.
	/// </summary>
	public IEnumerable<string>? OrderByFields { get; private set; }

	/// <summary>
	/// Режим сортировки.
	/// </summary>
	public string? OrderByMode { get; private set; }

	/// <summary>
	/// Номер страницы.
	/// </summary>
	public int? Page { get; private set; }

	/// <summary>
	/// Размер страницы.
	/// </summary>
	public int? PageSize { get; private set; }

	/// <summary>
	/// Текст для поиска.
	/// </summary>
	public string? TextSearch { get; private set; }

	[JsonIgnore]
	private Dictionary<string, string> IncludedInformationKeys =>
		IncludedInformation is null
			? []
			: _includedInformationKeys_cached ??= IncludedInformation.ToHashSet().ToDictionary(i => i, i => i);

	/// <summary>
	/// Устанавливает поля для сортировки.
	/// </summary>
	/// <param name="orderByFields">Поля для сортировки.</param>
	/// <returns>Обновленный запрос с установленными полями для сортировки.</returns>
	public GetCategoriesQuery WithOrderByFields(IEnumerable<string>? orderByFields)
	{
		return Copy(this, orderByFields: orderByFields);
	}

	/// <summary>
	/// Устанавливает режим сортировки.
	/// </summary>
	/// <param name="orderMode">Режим сортировки.</param>
	/// <returns>Обновленный запрос с установленным режимом сортировки.</returns>
	public GetCategoriesQuery WithOrderMode(string? orderMode)
	{
		return Copy(this, orderByMode: orderMode);
	}

	/// <summary>
	/// Устанавливает номер страницы.
	/// </summary>
	/// <param name="page">Номер страницы.</param>
	/// <returns>Обновленный запрос с установленным номером страницы.</returns>
	public GetCategoriesQuery WithPage(int? page)
	{
		return Copy(this, page: page);
	}

	/// <summary>
	/// Устанавливает размер страницы.
	/// </summary>
	/// <param name="pageSize">Размер страницы.</param>
	/// <returns>Обновленный запрос с установленным размером страницы.</returns>
	public GetCategoriesQuery WithPageSize(int? pageSize)
	{
		return Copy(this, pageSize: pageSize);
	}

	/// <summary>
	/// Устанавливает текст для поиска.
	/// </summary>
	/// <param name="textSearch">Текст для поиска.</param>
	/// <returns>Обновленный запрос с установленным текстом для поиска.</returns>
	public GetCategoriesQuery WithTextSearch(string? textSearch)
	{
		return Copy(this, textSearch: textSearch);
	}

	/// <summary>
	/// Устанавливает включаемую информацию.
	/// </summary>
	/// <param name="includedInformation">Включаемая информация.</param>
	/// <returns>Обновленный запрос с установленной включаемой информацией.</returns>
	public GetCategoriesQuery WithIncludedInformation(IEnumerable<string>? includedInformation)
	{
		return Copy(this, includedInformation: includedInformation);
	}

	/// <summary>
	/// Устанавливает идентификатор категории.
	/// </summary>
	/// <param name="id">Идентификатор категории.</param>
	/// <returns>Обновленный запрос с установленным идентификатором категории.</returns>
	public GetCategoriesQuery ForId(Guid? id)
	{
		return id == null || id == Guid.Empty ? this : Copy(this, id: id);
	}

	/// <summary>
	/// Устанавливает название категории.
	/// </summary>
	/// <param name="name">Название категории.</param>
	/// <returns>Обновленный запрос с установленным названием категории.</returns>
	public GetCategoriesQuery ForName(string? name)
	{
		return string.IsNullOrWhiteSpace(name) ? this : Copy(this, name: name);
	}

	/// <summary>
	/// Устанавливает идентификатор бренда.
	/// </summary>
	/// <param name="brandId">Идентификатор бренда.</param>
	/// <returns>Обновленный запрос с установленным идентификатором бренда.</returns>
	public GetCategoriesQuery ForBrandId(Guid? brandId)
	{
		return brandId == null || brandId == Guid.Empty ? this : Copy(this, brandId: brandId);
	}

	/// <summary>
	/// Устанавливает название бренда.
	/// </summary>
	/// <param name="brandName">Название бренда.</param>
	/// <returns>Обновленный запрос с установленным названием бренда.</returns>
	public GetCategoriesQuery ForBrandName(string? brandName)
	{
		return string.IsNullOrWhiteSpace(brandName) ? this : Copy(this, brandName: brandName);
	}

	/// <summary>
	/// Устанавливает идентификатор модели.
	/// </summary>
	/// <param name="modelId">Идентификатор модели.</param>
	/// <returns>Обновленный запрос с установленным идентификатором модели.</returns>
	public GetCategoriesQuery ForModelId(Guid? modelId)
	{
		return modelId == null || modelId == Guid.Empty ? this : Copy(this, modelId: modelId);
	}

	/// <summary>
	/// Устанавливает название модели.
	/// </summary>
	/// <param name="modelName">Название модели.</param>
	/// <returns>Обновленный запрос с установленным названием модели.</returns>
	public GetCategoriesQuery ForModelName(string? modelName)
	{
		return string.IsNullOrWhiteSpace(modelName) ? this : Copy(this, modelName: modelName);
	}

	/// <summary>
	/// Преобразует запрос в строковое представление в формате JSON.
	/// </summary>
	/// <returns>Строковое представление запроса в формате JSON.</returns>
	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}

	/// <summary>
	/// Проверяет, содержит ли включаемая информация указанный ключ.
	/// </summary>
	/// <param name="key">Ключ для проверки.</param>
	/// <returns>True, если включаемая информация содержит указанный ключ; в противном случае false.</returns>
	public bool ContainsIncludedInformationKey(string key)
	{
		return IncludedInformationKeys.ContainsKey(key);
	}

	private static GetCategoriesQuery Copy(
		GetCategoriesQuery origin,
		Guid? brandId = null,
		string? brandName = null,
		Guid? modelId = null,
		string? modelName = null,
		Guid? id = null,
		string? name = null,
		IEnumerable<string>? includedInformation = null,
		int? page = null,
		int? pageSize = null,
		string? textSearch = null,
		string? orderByMode = null,
		IEnumerable<string>? orderByFields = null
	)
	{
		return new()
		{
			BrandId = brandId ?? origin.BrandId,
			BrandName = brandName ?? origin.BrandName,
			ModelId = modelId ?? origin.ModelId,
			ModelName = modelName ?? origin.ModelName,
			Id = id ?? origin.Id,
			Name = name ?? origin.Name,
			IncludedInformation = includedInformation is null
				? origin.IncludedInformation
				: [.. includedInformation, .. origin.IncludedInformation ?? []],
			Page = page ?? origin.Page,
			PageSize = pageSize ?? origin.PageSize,
			TextSearch = textSearch ?? origin.TextSearch,
			OrderByMode = orderByMode ?? origin.OrderByMode,
			OrderByFields = orderByFields ?? origin.OrderByFields,
		};
	}
}
