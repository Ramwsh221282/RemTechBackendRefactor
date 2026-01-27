using System.Text.Json;
using System.Text.Json.Serialization;
using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Locations.Queries;

/// <summary>
/// Запрос на получение локаций транспортных средств.
/// </summary>
public sealed class GetLocationsQuery : IQuery
{
	[JsonIgnore]
	private Dictionary<string, string>? _includedInformationKeys_cached;

	// private constructor. Use static methods for creation.
	private GetLocationsQuery() { }

	/// <summary>
	/// Количество локаций для получения.
	/// </summary>
	public int? Amount { get; private init; } = 20;

	/// <summary>
	/// Текстовый поиск.
	/// </summary>
	public string? TextSearch { get; private init; }

	/// <summary>
	/// Идентификатор локации.
	/// </summary>
	public Guid? Id { get; private init; }

	/// <summary>
	/// Идентификатор категории.
	/// </summary>
	public Guid? CategoryId { get; private init; }

	/// <summary>
	/// Идентификатор бренда.
	/// </summary>
	public Guid? BrandId { get; private init; }

	/// <summary>
	/// Идентификатор модели.
	/// </summary>
	public Guid? ModelId { get; private init; }

	/// <summary>
	/// Название категории.
	/// </summary>
	public string? CategoryName { get; private init; }

	/// <summary>
	/// Название бренда.
	/// </summary>
	public string? BrandName { get; private init; }

	/// <summary>
	/// Название модели.
	/// </summary>
	public string? ModelName { get; private init; }

	/// <summary>
	/// Включаемые дополнительные сведения.
	/// </summary>
	public IEnumerable<string>? Includes { get; private init; }

	/// <summary>
	/// Сортировка по имени.
	/// </summary>
	public bool? UseOrderByName { get; private init; }

	[JsonIgnore]
	private Dictionary<string, string> IncludedInformationKeys =>
		_includedInformationKeys_cached ??= Includes is null ? [] : Includes.ToHashSet().ToDictionary(i => i, i => i);

	/// <summary>
	/// Создает новый экземпляр запроса на получение локаций с значением по умолчанию.
	/// </summary>
	/// <returns>Новый экземпляр запроса на получение локаций с значением по умолчанию.</returns>
	public static GetLocationsQuery Create() => new() { Amount = 20 };

	/// <summary>
	/// Создает новый экземпляр запроса на получение локаций с указанным идентификатором.
	/// </summary>
	/// <param name="id">Идентификатор локации.</param>
	/// <returns>Новый экземпляр запроса на получение локаций с указанным идентификатором.</returns>
	public GetLocationsQuery WithId(Guid? id) => Copy(this, id: id);

	/// <summary>
	/// Создает новый экземпляр запроса на получение локаций с указанным количеством.
	/// </summary>
	/// <param name="amount">Количество локаций для получения.</param>
	/// <returns>Новый экземпляр запроса на получение локаций с указанным количеством.</returns>
	public GetLocationsQuery WithAmount(int? amount) => Copy(this, amount: amount);

	/// <summary>
	/// Создает новый экземпляр запроса на получение локаций с указанным идентификатором категории.
	/// </summary>
	/// <param name="categoryId">Идентификатор категории.</param>
	/// <returns>Новый экземпляр запроса на получение локаций с указанным идентификатором категории.</returns>
	public GetLocationsQuery WithCategoryId(Guid? categoryId) => Copy(this, categoryId: categoryId);

	/// <summary>
	/// Создает новый экземпляр запроса на получение локаций с указанным идентификатором бренда.
	/// </summary>
	/// <param name="brandId">Идентификатор бренда.</param>
	/// <returns>Новый экземпляр запроса на получение локаций с указанным идентификатором бренда.</returns>
	public GetLocationsQuery WithBrandId(Guid? brandId) => Copy(this, brandId: brandId);

	/// <summary>
	/// Создает новый экземпляр запроса на получение локаций с указанным идентификатором модели.
	/// </summary>
	/// <param name="modelId">Идентификатор модели.</param>
	/// <returns>Новый экземпляр запроса на получение локаций с указанным идентификатором модели.</returns>
	public GetLocationsQuery WithModelId(Guid? modelId) => Copy(this, modelId: modelId);

	/// <summary>
	/// Создает новый экземпляр запроса на получение локаций с указанием сортировки по имени.
	/// </summary>
	/// <param name="useOrderByName">Флаг, указывающий, следует ли сортировать по имени.</param>
	/// <returns>Новый экземпляр запроса на получение локаций с указанным параметром сортировки по имени.</returns>
	public GetLocationsQuery WithOrderByName(bool? useOrderByName) => Copy(this, orderByName: useOrderByName);

	/// <summary>
	/// Создает новый экземпляр запроса на получение локаций с указанным названием категории.
	/// </summary>
	/// <param name="categoryName">Название категории.</param>
	/// <returns>Новый экземпляр запроса на получение локаций с указанным названием категории.</returns>
	public GetLocationsQuery WithCategoryName(string? categoryName) => Copy(this, categoryName: categoryName);

	/// <summary>
	/// Создает новый экземпляр запроса на получение локаций с указанным названием бренда.
	/// </summary>
	/// <param name="brandName">Название бренда.</param>
	/// <returns>Новый экземпляр запроса на получение локаций с указанным названием бренда.</returns>
	public GetLocationsQuery WithBrandName(string? brandName) => Copy(this, brandName: brandName);

	/// <summary>
	/// Создает новый экземпляр запроса на получение локаций с указанным названием модели.
	/// </summary>
	/// <param name="modelName">Название модели.</param>
	/// <returns>Новый экземпляр запроса на получение локаций с указанным названием модели.</returns>
	public GetLocationsQuery WithModelName(string? modelName) => Copy(this, modelName: modelName);

	/// <summary>
	/// Создает новый экземпляр запроса на получение локаций с указанным текстовым поиском.
	/// </summary>
	/// <param name="textSearch">Текст для поиска.</param>
	/// <returns>Новый экземпляр запроса на получение локаций с указанным текстовым поиском.</returns>
	public GetLocationsQuery WithTextSearch(string? textSearch) => Copy(this, textSearch: textSearch);

	/// <summary>
	/// Создает новый экземпляр запроса на получение локаций с указанными включаемыми полями.
	/// </summary>
	/// <param name="includes">Перечисление включаемых полей.</param>
	/// <returns>Новый экземпляр запроса на получение локаций с указанными включаемыми полями.</returns>
	public GetLocationsQuery WithIncludes(IEnumerable<string>? includes) => Copy(this, includes: includes);

	/// <summary>
	/// Проверяет, содержит ли запрос указанное включаемое поле.
	/// </summary>
	/// <param name="includeName">Название включаемого поля.</param>
	/// <returns>True, если запрос содержит указанное включаемое поле; в противном случае false.</returns>
	public bool ContainsInclude(string includeName) => IncludedInformationKeys.ContainsKey(includeName);

	/// <summary>
	/// Проверяет, содержит ли запрос фильтр по категории.
	/// </summary>
	/// <returns>True, если запрос содержит фильтр по категории; в противном случае false.</returns>
	public bool ContainsCategoryFilter() =>
		(CategoryId != null && CategoryId.Value != Guid.Empty) || !string.IsNullOrWhiteSpace(CategoryName);

	/// <summary>
	/// Проверяет, содержит ли запрос фильтр по бренду.
	/// </summary>
	/// <returns>True, если запрос содержит фильтр по бренду; в противном случае false.</returns>
	public bool ContainsBrandFilter() =>
		(BrandId != null && BrandId.Value != Guid.Empty) || !string.IsNullOrWhiteSpace(BrandName);

	/// <summary>
	/// Проверяет, содержит ли запрос фильтр по модели.
	/// </summary>
	/// <returns>True, если запрос содержит фильтр по модели; в противном случае false.</returns>
	public bool ContainsModelFilter() =>
		(ModelId != null && ModelId.Value != Guid.Empty) || !string.IsNullOrWhiteSpace(ModelName);

	/// <summary>
	/// Преобразует текущий объект запроса в его строковое представление в формате JSON.
	/// </summary>
	/// <returns>Строковое представление объекта запроса в формате JSON.</returns>
	public override string ToString() => JsonSerializer.Serialize(this);

	/// <summary>
	/// Создает копию текущего запроса с возможностью изменения отдельных свойств.
	/// </summary>
	/// <param name="original">Исходный объект запроса для копирования.</param>
	/// <param name="amount">Количество элементов для получения.</param>
	/// <param name="textSearch">Текст для поиска.</param>
	/// <param name="id">Идентификатор локации.</param>
	/// <param name="categoryId">Идентификатор категории.</param>
	/// <param name="brandId">Идентификатор бренда.</param>
	/// <param name="modelId">Идентификатор модели.</param>
	/// <param name="categoryName">Название категории.</param>
	/// <param name="brandName">Название бренда.</param>
	/// <param name="modelName">Название модели.</param>
	/// <param name="includes">Перечисление включаемых полей.</param>
	/// <param name="orderByName">Флаг сортировки по имени.</param>
	/// <returns>Новый экземпляр запроса на получение локаций с измененными свойствами.</returns>
	private static GetLocationsQuery Copy(
		GetLocationsQuery original,
		int? amount = null,
		string? textSearch = null,
		Guid? id = null,
		Guid? categoryId = null,
		Guid? brandId = null,
		Guid? modelId = null,
		string? categoryName = null,
		string? brandName = null,
		string? modelName = null,
		IEnumerable<string>? includes = null,
		bool? orderByName = null
	) =>
		new()
		{
			Amount = amount ?? original.Amount,
			TextSearch = textSearch ?? original.TextSearch,
			Id = id ?? original.Id,
			CategoryId = categoryId ?? original.CategoryId,
			BrandId = brandId ?? original.BrandId,
			ModelId = modelId ?? original.ModelId,
			CategoryName = categoryName ?? original.CategoryName,
			BrandName = brandName ?? original.BrandName,
			ModelName = modelName ?? original.ModelName,
			Includes = includes is null ? original.Includes : [.. includes],
			UseOrderByName = orderByName ?? original.UseOrderByName,
		};
}
