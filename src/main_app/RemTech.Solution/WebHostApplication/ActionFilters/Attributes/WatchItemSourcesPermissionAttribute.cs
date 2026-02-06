using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters.AuthFilters;

namespace WebHostApplication.ActionFilters.Attributes;

/// <summary>
/// Атрибут для проверки наличия разрешения на просмотр источников элементов наблюдения.
/// </summary>
public sealed class WatchItemSourcesPermissionAttribute : TypeFilterAttribute
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="WatchItemSourcesPermissionAttribute"/>.
	/// </summary>
	public WatchItemSourcesPermissionAttribute()
		: base(typeof(ShouldHaveWatchItemSourcesPermissionFilter)) { }
}
