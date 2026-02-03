using System.Collections.ObjectModel;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Telemetry.Core.ActionRecords.ValueObjects;
using WebHostApplication.ActionFilters.Common;

namespace WebHostApplication.Middlewares.Telemetry;

/// <summary>
/// Поведение для извлечения имени записи действия и полезной нагрузки из HttpContext.
/// </summary>
public static class HttpContextActionRecordsParsing
{
	private static readonly string[] _ignoredKeyForRouterParameters = ["action", "controller"];

	extension(ActionExecutingContext context)
	{
		public Result<ActionRecordName> ExtractRecordName()
		{
			return context.HttpContext.ExtractRecordName();
		}

		public Task<ActionRecordPayloadJson?> ExtractPayload(Serilog.ILogger? logger = null)
		{
			return context.HttpContext.ExtractPayload(logger);
		}
	}

	extension(HttpContext context)
	{
		public Result<ActionRecordName> ExtractRecordName()
		{
			string basePath = context.Request.PathBase.Value ?? string.Empty;
			string path = context.Request.Path.Value ?? string.Empty;
			return ActionRecordName.Create(basePath + path);
		}

		public async Task<ActionRecordPayloadJson?> ExtractPayload(Serilog.ILogger? logger = null)
		{
			ReadOnlyDictionary<string, List<object?>> routeParameters = GetRouteParametersFromHttpContext(context);
			ReadOnlyDictionary<string, List<object>> queryParameters = GetQueryParametersFromHttpContext(context);
			ReadOnlyDictionary<string, string> payloadParameters = await GetPayloadParametersFromHttpContext(
				context,
				logger
			);

			if (queryParameters.Count is 0 && payloadParameters.Count is 0 && routeParameters.Count is 0)
			{
				return null;
			}

			string jsonPayload = JsonSerializer.Serialize(
				new
				{
					QueryParameters = queryParameters,
					PayloadParameters = payloadParameters,
					RouteParameters = routeParameters,
				}
			);

			return ActionRecordPayloadJson.Create(jsonPayload);
		}

		private ReadOnlyDictionary<string, List<object?>> GetRouteParametersFromHttpContext()
		{
			Dictionary<string, List<object?>> result = [];
			foreach (KeyValuePair<string, object?> entry in context.Request.RouteValues)
			{
				string key = entry.Key;
				if (ContainsInIgnoredRouteParameters(key))
				{
					continue;
				}

				if (!result.TryGetValue(key, out List<object?>? existing))
				{
					existing = [];
					result.Add(key, existing);
				}

				result[key].Add(entry.Value);
			}

			return result.AsReadOnly();
		}

		private ReadOnlyDictionary<string, List<object>> GetQueryParametersFromHttpContext()
		{
			if (!context.Request.QueryString.HasValue)
			{
				return ReadOnlyDictionary<string, List<object>>.Empty;
			}

			Dictionary<string, List<object>> result = [];
			foreach (KeyValuePair<string, StringValues> entry in context.Request.Query)
			{
				string key = entry.Key;
				if (!result.TryGetValue(key, out List<object>? existing))
				{
					existing = [];
					result.Add(key, existing);
				}

				result[key].AddRange(entry.Value);
			}

			return result.AsReadOnly();
		}

		private async Task<ReadOnlyDictionary<string, string>> GetPayloadParametersFromHttpContext(
			Serilog.ILogger? logger = null
		)
		{
			if (!context.Request.Body.CanSeek)
			{
				return ReadOnlyDictionary<string, string>.Empty;
			}

			try
			{
				Dictionary<string, string>? payload = await context.Request.ReadFromJsonAsync<
					Dictionary<string, string>
				>(cancellationToken: context.CancellationToken);
				return payload != null ? payload.AsReadOnly() : ReadOnlyDictionary<string, string>.Empty;
			}
			catch (Exception ex)
			{
				logger?.Error(
					ex,
					"Не удалось прочитать тело запроса для получения параметров полезной нагрузки из HttpContext."
				);
				return ReadOnlyDictionary<string, string>.Empty;
			}
		}
	}

	private static bool ContainsInIgnoredRouteParameters(string input)
	{
		foreach (string ignoredKey in _ignoredKeyForRouterParameters)
		{
			if (string.Equals(input, ignoredKey, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		return false;
	}
}
