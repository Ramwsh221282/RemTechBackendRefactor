using System.Net;
using Identity.Domain.Accounts.Features.VerifyToken;
using Microsoft.AspNetCore.Mvc.Filters;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using WebHostApplication.ActionFilters.Common;

namespace WebHostApplication.ActionFilters.Filters;

public sealed class VerifyTokenFilter(ICommandHandler<VerifyTokenCommand, Unit> handler) : IAsyncActionFilter
{
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

	private async Task<Result<Unit>> VerifyToken(ActionExecutingContext context, CancellationToken ct)
	{
		string token = context.HttpContext.GetAccessToken([
			httpContext => httpContext.GetAccessTokenFromHeaderOrEmpty(),
			httpContext => httpContext.GetAccessTokenFromCookieOrEmpty(),
		]);

		VerifyTokenCommand command = new(token);
		return await handler.Execute(command, ct);
	}

	private static async Task WriteResultToHttpContext(
		ActionExecutingContext context,
		Envelope envelope,
		CancellationToken ct
	)
	{
		context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
		context.HttpContext.Response.ContentType = "application/json";
		await context.HttpContext.Response.WriteAsJsonAsync(envelope, ct);
	}

	private static Envelope UnauthorizedEnvelope(Result<Unit> failure) =>
		new((int)HttpStatusCode.Unauthorized, null, failure.Error.Message);
}
