using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters.AuthFilters;

namespace WebHostApplication.ActionFilters.Attributes;

/// <summary>
/// Атрибут для проверки наличия разрешения на управление парсерами.
/// </summary>
public sealed class ParserManagementPermissionAttribute : TypeFilterAttribute
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="ParserManagementPermissionAttribute"/>.
	/// </summary>
	public ParserManagementPermissionAttribute()
		: base(typeof(ShouldHaveParserManagementPermissionFilter)) { }
}
