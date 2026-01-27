using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters;

namespace WebHostApplication.ActionFilters.Attributes;

/// <summary>
/// Атрибут для проверки наличия разрешения на добавление элементов в избранное.
/// </summary>
public sealed class AddItemsToFavoritesPermissionAttribute : TypeFilterAttribute
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="AddItemsToFavoritesPermissionAttribute"/>.
	/// </summary>
	public AddItemsToFavoritesPermissionAttribute()
		: base(typeof(ShouldHaveAddItemsToFavoritesPermissionFilter)) { }
}
