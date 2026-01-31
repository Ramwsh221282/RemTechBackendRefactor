using Identity.Domain.Tokens;
using Identity.Infrastructure.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace WebHostApplication.ActionFilters.Filters.TelemetryFilters;

/// <summary>
/// Искатель идентификатора вызывающего объекта для записей телеметрии.
/// </summary>
/// <param name="options">Опции JWT.</param>
/// <param name="logger">Логгер для записи информации.</param>
public sealed class TelemetryRecordInvokerIdSearcher(IOptions<JwtOptions> options, Serilog.ILogger logger)
{
	private JwtTokenManager Manager { get; } = new(options, logger);
	private Serilog.ILogger Logger { get; } = logger.ForContext<TelemetryRecordInvokerIdSearcher>();

	/// <summary>
	/// Пытается найти идентификатор вызывающего объекта из HttpContext.
	/// </summary>
	/// <param name="context">Контекст выполнения действия.</param>
	/// <returns>Идентификатор вызывающего объекта, если найден.</returns>
	public Optional<Guid> TryReadInvokerIdToken(ActionExecutingContext context) =>
		TryReadInvokerIdToken(context.HttpContext);

	/// <summary>
	/// Пытается найти идентификатор вызывающего объекта из HttpContext.
	/// </summary>
	/// <param name="context">Контекст выполнения действия.</param>
	/// <returns>Идентификатор вызывающего объекта, если найден.</returns>
	public Optional<Guid> TryReadInvokerIdToken(HttpContext context)
	{
		string? maybeToken = ExtractAccessTokenFromHttpContext(context);
		return ReadTokenUsingJwtManager(maybeToken);
	}

	private static string? ExtractAccessTokenFromHttpContext(HttpContext context)
	{
		string? token = context.GetAccessToken();
		return string.IsNullOrWhiteSpace(token) ? null : token;
	}

	private Optional<Guid> ReadTokenUsingJwtManager(string? token)
	{
		if (string.IsNullOrWhiteSpace(token))
			return Optional.None<Guid>();

		try
		{
			AccessToken reading = Manager.ReadToken(token);
			return reading.UserId;
		}
		catch (Exception ex)
		{
			Logger.Error(ex, "Error at reading token.");
			return Optional.None<Guid>();
		}
	}
}
