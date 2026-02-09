using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters.AuthFilters;

namespace WebHostApplication.ActionFilters.Attributes;

/// <summary>
/// Атрибут для проверки наличия разрешения на управление идентификацией.
/// </summary>
public sealed class IdentityManagementPermissionAttribute : TypeFilterAttribute
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="IdentityManagementPermissionAttribute"/>.
	/// </summary>
	public IdentityManagementPermissionAttribute()
		: base(typeof(ShouldHaveIdentityManagementPermissionFilter)) { }
}
