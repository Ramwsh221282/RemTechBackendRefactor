using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters;

namespace WebHostApplication.ActionFilters.Attributes;

/// <summary>
/// Атрибут для проверки валидности токена.
/// </summary>
public sealed class VerifyTokenAttribute : TypeFilterAttribute
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="VerifyTokenAttribute"/>.
	/// </summary>
	public VerifyTokenAttribute()
		: base(typeof(VerifyTokenFilter)) { }
}
