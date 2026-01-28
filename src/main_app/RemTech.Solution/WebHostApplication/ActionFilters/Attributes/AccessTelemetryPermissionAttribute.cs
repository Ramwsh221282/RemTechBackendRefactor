using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters.AuthFilters;

namespace WebHostApplication.ActionFilters.Attributes;

/// <summary>
/// Атрибут для проверки наличия разрешения на доступ к телеметрии.
/// </summary>
public sealed class AccessTelemetryPermissionAttribute : TypeFilterAttribute
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="AccessTelemetryPermissionAttribute"/>.
	/// </summary>
	public AccessTelemetryPermissionAttribute()
		: base(typeof(ShouldHaveAccessTelemetryPermissionFilter)) { }
}
