using Microsoft.AspNetCore.Mvc.Filters;
using WebHostApplication.ActionFilters.Common;

namespace WebHostApplication.Middlewares.Telemetry;

/// <summary>
/// Поведение для получения токенов из HttpContext.
/// </summary>
public static class HttpContextTokensBehavior
{
	extension(HttpContext context)
	{
		public string? GetAccessToken()
		{
			return _getAccessTokenMethods
				.Select(method => method(context))
				.FirstOrDefault(token => !string.IsNullOrWhiteSpace(token));
		}

		public string? GetRefreshToken()
		{
			return _getRefreshTokenMethods
				.Select(method => method(context))
				.FirstOrDefault(token => !string.IsNullOrWhiteSpace(token));
		}
	}

	extension(ActionExecutingContext context)
	{
		public string? GetAccessToken()
		{
			return context.HttpContext.GetAccessToken();
		}

		public string? GetRefreshToken()
		{
			return context.HttpContext.GetRefreshToken();
		}
	}

	extension(ActionExecutedContext context)
	{
		public string? GetAccessToken()
		{
			return context.HttpContext.GetAccessToken();
		}

		public string? GetRefreshToken()
		{
			return context.HttpContext.GetRefreshToken();
		}
	}

	private static readonly Func<HttpContext, string>[] _getAccessTokenMethods =
	[
		context => context.GetAccessTokenFromHeaderOrEmpty(),
		context => context.GetAccessTokenFromCookieOrEmpty(),
	];

	private static readonly Func<HttpContext, string>[] _getRefreshTokenMethods =
	[
		context => context.GetRefreshTokenFromHeaderOrEmpty(),
		context => context.GetRefreshTokenFromCookieOrEmpty(),
	];
}
