using System.Net;
using Identity.Domain.Accounts.Features.VerifyToken;
using Microsoft.AspNetCore.Mvc.Filters;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using WebHostApplication.ActionFilters.Common;

namespace WebHostApplication.ActionFilters.Filters;

/// <summary>
/// Фильтр для проверки валидности токена доступа.
/// </summary>
/// <param name="handler"> Обработчик команды проверки токена. </param>
public sealed class VerifyTokenFilter(ICommandHandler<VerifyTokenCommand, Unit> handler) : IAsyncActionFilter
{
	/// <summary>
	/// Вызывается перед выполнением действия контроллера.
	/// </summary>
	/// <param name="context"> Контекст выполнения действия. </param>
	/// <param name="next"> Делегат для вызова следующего фильтра или действия. </param>
	/// <returns> Задача, представляющая асинхронную операцию. </returns>
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		CancellationToken ct = context.HttpContext.RequestAborted;
		Result<Unit> result = await VerifyToken(context, ct);

		if (result.IsFailure)
		{
			Envelope envelope = UnauthorizedEnvelope(result);
			await WriteResultToHttpContext(context, envelope, ct);
			return;
		}

		await next();
	}

	private static Task WriteResultToHttpContext(
		ActionExecutingContext context,
		Envelope envelope,
		CancellationToken ct
	)
	{
		context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
		context.HttpContext.Response.ContentType = "application/json";
		return context.HttpContext.Response.WriteAsJsonAsync(envelope, ct);
	}

	private static Envelope UnauthorizedEnvelope(Result<Unit> failure) =>
		new((int)HttpStatusCode.Unauthorized, null, failure.Error.Message);

	private Task<Result<Unit>> VerifyToken(ActionExecutingContext context, CancellationToken ct)
	{
		string token = context.HttpContext.GetAccessToken([
			httpContext => httpContext.GetAccessTokenFromHeaderOrEmpty(),
			httpContext => httpContext.GetAccessTokenFromCookieOrEmpty(),
		]);

		VerifyTokenCommand command = new(token);
		return handler.Execute(command, ct);
	}
}
