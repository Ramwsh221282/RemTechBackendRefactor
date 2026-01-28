using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters.AuthFilters;

namespace WebHostApplication.ActionFilters.Attributes;

/// <summary>
/// Атрибут для проверки наличия разрешения на управление уведомлениями.
/// </summary>
public sealed class NotificationsManagementPermissionAttribute : TypeFilterAttribute
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="NotificationsManagementPermissionAttribute"/>.
	/// </summary>
	public NotificationsManagementPermissionAttribute()
		: base(typeof(ShouldHaveNotificationsManagementPermissionFilter)) { }
}
