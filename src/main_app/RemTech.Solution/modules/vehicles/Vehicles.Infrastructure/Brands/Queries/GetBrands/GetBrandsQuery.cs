using System.Text.Json;
using System.Text.Json.Serialization;
using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Brands.Queries.GetBrands;

/// <summary>
/// Запрос получения брендов.
/// </summary>
public sealed class GetBrandsQuery : IQuery
{
	[JsonIgnore]
	private Dictionary<string, string>? _includedInformationKeys_cached;

	/// <summary>
	/// Идентификатор категории транспортного средства.
	/// </summary>
	public Guid? CategoryId { get; private init; }

	/// <summary>
	/// Название категории транспортного средства.
	/// </summary>
	public string? CategoryName { get; private init; }

	/// <summary>
	/// Идентификатор модели транспортного средства.
	/// </summary>
	public Guid? ModelId { get; private init; }

	/// <summary>
	/// Название модели транспортного средства.
	/// </summary>
	public string? ModelName { get; private init; }

	/// <summary>
	/// Идентификатор бренда.
	/// </summary>
	public Guid? Id { get; private init; }

	/// <summary>
	/// Название бренда.
	/// </summary>
	public string? Name { get; private init; }

	/// <summary>
	/// Включаемая информация в ответ.
	/// </summary>
	public IEnumerable<string>? Includes { get; private init; }

	/// <summary>
	/// Номер страницы.
	/// </summary>
	public int? Page { get; private init; }

	/// <summary>
	/// Размер страницы.
	/// </summary>
	public int? PageSize { get; private init; }

	/// <summary>
	/// Текстовый поиск.
	/// </summary>
	public string? TextSearch { get; private init; }

	/// <summary>
	/// Направление сортировки.
	/// </summary>
	public string? SortMode { get; private init; }

	/// <summary>
	/// Поля для сортировки.
	/// </summary>
	public IEnumerable<string>? SortFields { get; private init; }

	[JsonIgnore]
	private Dictionary<string, string> IncludedInformationKeys =>
		_includedInformationKeys_cached ??= Includes is null ? [] : Includes.ToHashSet().ToDictionary(i => i, i => i);

	/// <summary>
	/// Проверяет, содержит ли запрос указанную включаемую информацию.
	/// </summary>
	/// <param name="include"> Включаемая информация для проверки. </param>
	/// <returns> Возвращает true, если включаемая информация присутствует в запросе; в противном случае false. </returns>
	public bool ContainsFieldInclude(string include) => IncludedInformationKeys.ContainsKey(include);

	/// <summary>
	/// Устанавливает направление сортировки.
	/// </summary>
	/// <param name="sortDirection"> Направление сортировки. </param>
	/// <returns> Возвращает новый экземпляр запроса с установленным направлением сортировки. </returns>
	public GetBrandsQuery WithSortDirection(string? sortDirection) => Copy(this, sortDirection: sortDirection);

	/// <summary>
	/// Устанавливает поля для сортировки.
	/// </summary>
	/// <param name="sortFields"> Поля для сортировки. </param>
	/// <returns> Возвращает новый экземпляр запроса с установленными полями для сортировки. </returns>
	public GetBrandsQuery WithSortFields(IEnumerable<string>? sortFields) => Copy(this, sortFields: sortFields);

	/// <summary>
	/// Устанавливает включаемую информацию.
	/// </summary>
	/// <param name="includes"> Включаемая информация. </param>
	/// <returns> Возвращает новый экземпляр запроса с установленной включаемой информацией. </returns>
	public GetBrandsQuery WithInclude(IEnumerable<string>? includes) =>
		includes?.Any() != true ? this : Copy(this, includes: includes);

	/// <summary>
	/// Устанавливает номер страницы.
	/// </summary>
	/// <param name="page"> Номер страницы. </param>
	/// <returns> Возвращает новый экземпляр запроса с установленным номером страницы. </returns>
	public GetBrandsQuery WithPagination(int? page) => page == null || page <= 0 ? this : Copy(this, page: page);

	/// <summary>
	/// Устанавливает размер страницы.
	/// </summary>
	/// <param name="pageSize"> Размер страницы. </param>
	/// <returns> Возвращает новый экземпляр запроса с установленным размером страницы. </returns>
	public GetBrandsQuery WithPageSize(int? pageSize) =>
		pageSize == null || pageSize >= 30 ? this : Copy(this, pageSize: pageSize);

	/// <summary>
	/// Устанавливает текстовый поиск.
	/// </summary>
	/// <param name="textSearch"> Текст для поиска. </param>
	/// <returns> Возвращает новый экземпляр запроса с установленным текстовым поиском. </returns>
	public GetBrandsQuery WithTextSearch(string? textSearch) =>
		string.IsNullOrWhiteSpace(textSearch) ? this : Copy(this, textSearch: textSearch);

	/// <summary>
	/// Устанавливает идентификатор бренда.
	/// </summary>
	/// <param name="id"> Идентификатор бренда. </param>
	/// <returns> Возвращает новый экземпляр запроса с установленным идентификатором бренда. </returns>
	public GetBrandsQuery ForId(Guid? id) => id == null || id == Guid.Empty ? this : Copy(this, id: id);

	/// <summary>
	/// Устанавливает название бренда.
	/// </summary>
	/// <param name="name"> Название бренда. </param>
	/// <returns> Возвращает новый экземпляр запроса с установленным названием бренда. </returns>
	public GetBrandsQuery ForName(string? name) => string.IsNullOrWhiteSpace(name) ? this : Copy(this, name: name);

	/// <summary>
	/// Устанавливает идентификатор категории транспортного средства.
	/// </summary>
	/// <param name="categoryId"> Идентификатор категории транспортного средства. </param>
	/// <returns> Возвращает новый экземпляр запроса с установленным идентификатором категории транспортного средства. </returns>
	public GetBrandsQuery ForCategoryId(Guid? categoryId) =>
		categoryId == null || categoryId == Guid.Empty ? this : Copy(this, categoryId: categoryId);

	/// <summary>
	/// Устанавливает название категории транспортного средства.
	/// </summary>
	/// <param name="categoryName"> Название категории транспортного средства. </param>
	/// <returns> Возвращает новый экземпляр запроса с установленным названием категории транспортного средства. </returns>
	public GetBrandsQuery ForCategoryName(string? categoryName) =>
		string.IsNullOrWhiteSpace(categoryName) ? this : Copy(this, categoryName: categoryName);

	/// <summary>
	/// Устанавливает идентификатор модели транспортного средства.
	/// </summary>
	/// <param name="modelId"> Идентификатор модели транспортного средства. </param>
	/// <returns> Возвращает новый экземпляр запроса с установленным идентификатором модели транспортного средства. </returns>
	public GetBrandsQuery ForModelId(Guid? modelId) =>
		modelId == null || modelId == Guid.Empty ? this : Copy(this, modelId: modelId);

	/// <summary>
	/// Устанавливает название модели транспортного средства.
	/// </summary>
	/// <param name="modelName"> Название модели транспортного средства. </param>
	/// <returns> Возвращает новый экземпляр запроса с установленным названием модели транспортного средства. </returns>
	public GetBrandsQuery ForModelName(string? modelName) =>
		string.IsNullOrWhiteSpace(modelName) ? this : Copy(this, modelName: modelName);

	/// <summary>
	/// Преобразует запрос в строковое представление в формате JSON.
	/// </summary>
	/// <returns> Строковое представление запроса в формате JSON. </returns>
	public override string ToString() => JsonSerializer.Serialize(this);

	private static GetBrandsQuery Copy(
		GetBrandsQuery origin,
		Guid? id = null,
		string? name = null,
		Guid? categoryId = null,
		string? categoryName = null,
		Guid? modelId = null,
		string? modelName = null,
		IEnumerable<string>? includes = null,
		int? page = null,
		int? pageSize = null,
		string? textSearch = null,
		string? sortDirection = null,
		IEnumerable<string>? sortFields = null
	) =>
		new()
		{
			CategoryId = categoryId ?? origin.CategoryId,
			CategoryName = categoryName ?? origin.CategoryName,
			ModelId = modelId ?? origin.ModelId,
			ModelName = modelName ?? origin.ModelName,
			Id = id ?? origin.Id,
			Name = name ?? origin.Name,
			Includes = includes ?? origin.Includes,
			Page = page ?? origin.Page,
			PageSize = pageSize ?? origin.PageSize,
			TextSearch = textSearch ?? origin.TextSearch,
			SortMode = sortDirection ?? origin.SortMode,
			SortFields = sortFields ?? origin.SortFields,
		};
}
