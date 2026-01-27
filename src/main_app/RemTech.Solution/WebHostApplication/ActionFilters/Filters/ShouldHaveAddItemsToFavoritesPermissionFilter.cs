using Identity.Domain.Contracts.Jwt;
using Microsoft.AspNetCore.Mvc.Filters;
using WebHostApplication.ActionFilters.Common;

namespace WebHostApplication.ActionFilters.Filters;

/// <summary>
/// Фильтр для проверки наличия разрешения на добавление элементов в избранное.
/// </summary>
/// <param name="manager"> Менеджер для работы с JWT токенами. </param>
public sealed class ShouldHaveAddItemsToFavoritesPermissionFilter(IJwtTokenManager manager) : IAsyncActionFilter
{
	/// <summary>
	/// Вызывается перед выполнением действия контроллера.
	/// </summary>
	/// <param name="context"> Контекст выполнения действия. </param>
	/// <param name="next"> Делегат для вызова следующего фильтра или действия. </param>
	/// <returns> Задача, представляющая асинхронную операцию. </returns>
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		if (!context.HttpContext.HasPermission("add.items.to.favorites", manager))
		{
			await context.HttpContext.WriteForbiddenResult();
			return;
		}

		await next();
	}
}
