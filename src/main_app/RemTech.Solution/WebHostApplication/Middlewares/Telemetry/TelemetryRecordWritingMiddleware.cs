using System.Text;
using System.Text.Json;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Telemetry.Core.ActionRecords;
using Telemetry.Core.ActionRecords.ValueObjects;
using Telemetry.Infrastructure;
using WebHostApplication.ActionFilters.Common;

namespace WebHostApplication.Middlewares.Telemetry;

/// <summary>
/// Middleware для записи действий телеметрии.
/// </summary>
/// <param name="idSearcher">Искатель идентификатора пользователя.</param>
/// <param name="actionRecords">Хранилище записей действий телеметрии.</param>
/// <param name="logger">Логгер для записи информации.</param>
/// <param name="next">Делегат для следующего действия в конвейере.</param>
public sealed class TelemetryRecordWritingMiddleware(
	TelemetryRecordInvokerIdSearcher idSearcher,
	RedisTelemetryActionsStorage actionRecords,
	Serilog.ILogger logger,
	RequestDelegate next
)
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<TelemetryRecordWritingMiddleware>();
	private RequestDelegate Next { get; } = next;

	/// <summary>
	/// Выполнить HTTP запрос и записать результаты выполнения HTTP запроса от пользователя в телеметрию.
	/// </summary>
	/// <param name="context">Контекст HTTP запроса.</param>
	/// <returns>Выполнение задачи Task.</returns>
	public async Task InvokeAsync(HttpContext context)
	{
		CancellationToken ct = context.CancellationToken;
		Stream bodyClone = context.Response.Body;

		await using MemoryStream buffer = CreateTemporaryBufferForProcessing(context);
		string resourceName = ResourseNameResolver.ResolveHumanizedResourceNameFromHttpContext(context);
		ActionRecordName name = ActionRecordName.Create(resourceName);
		ActionRecordPayloadJson? payload = await context.ExtractPayload(Logger);
		Optional<Guid> invokerId = idSearcher.TryReadInvokerIdToken(context);
		ActionRecordSeverity severity = await InvokeAndReturnSeverity(context);
		ActionRecordError? error = null;

		ResetBufferToBeginning(buffer);
		await CopyClonedStreamToResponse(buffer, bodyClone, context, ct);

		if (severity.Equals(ActionRecordSeverity.Error()) && buffer.Length > 0)
		{
			string errorText = await TryReadBusinessLogicErrorMessage(buffer, ct);
			error = ActionRecordError.FromNullableString(errorText);
		}

		ActionRecord action = ActionRecord.CreateNew(invokerId, name, payload, severity, error);
		actionRecords.WriteRecord(action);
	}

	private static void ResetBufferToBeginning(Stream stream)
	{
		stream.Seek(0, SeekOrigin.Begin);
	}

	private static MemoryStream CreateTemporaryBufferForProcessing(HttpContext context)
	{
		MemoryStream buffer = new();
		context.Response.Body = buffer;
		return buffer;
	}

	private static async Task CopyClonedStreamToResponse(
		Stream stream,
		Stream copy,
		HttpContext context,
		CancellationToken ct
	)
	{
		await stream.CopyToAsync(copy, ct);
		context.Response.Body = copy;
	}

	private static async Task<string> TryReadBusinessLogicErrorMessage(
		Stream bufferFromResponse,
		CancellationToken ct = default
	)
	{
		ResetBufferToBeginning(bufferFromResponse);
		using StreamReader reader = new(bufferFromResponse, Encoding.UTF8, leaveOpen: true);
		string message = await reader.ReadToEndAsync(ct);
		ResetBufferToBeginning(bufferFromResponse);
		using JsonDocument document = JsonDocument.Parse(message);
		if (!document.RootElement.TryGetProperty("message", out JsonElement messageField))
		{
			return string.Empty;
		}

		return (messageField.ValueKind == JsonValueKind.Null) ? string.Empty : messageField.GetString() ?? string.Empty;
	}

	private static ActionRecordSeverity ResolveSeverityByStatusCode(HttpContext context)
	{
		int statusCode = context.Response.StatusCode;
		return statusCode switch
		{
			>= 200 and < 400 => ActionRecordSeverity.Success(),
			>= 400 and < 500 => ActionRecordSeverity.Error(),
			>= 500 => ActionRecordSeverity.Warning(),
			_ => ActionRecordSeverity.Info(),
		};
	}

	private async Task<ActionRecordSeverity> InvokeAndReturnSeverity(HttpContext context)
	{
        await Next(context);
        return ResolveSeverityByStatusCode(context);
	}
}
