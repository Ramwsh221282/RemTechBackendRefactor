using System.Text;
using System.Text.Json;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Web;
using Telemetry.Core.ActionRecords;
using Telemetry.Core.ActionRecords.ValueObjects;
using WebHostApplication.ActionFilters.Common;

namespace WebHostApplication.ActionFilters.Filters.TelemetryFilters;

/// <summary>
/// Middleware для записи действий телеметрии.
/// </summary>
/// <param name="idSearcher">Искатель идентификатора пользователя.</param>
/// <param name="logger">Логгер для записи информации.</param>
/// <param name="next">Делегат для следующего действия в конвейере.</param>
public sealed class TelemetryRecordWritingMiddleware(
	TelemetryRecordInvokerIdSearcher idSearcher,
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
		if (severity.Equals(ActionRecordSeverity.Error()))
		{
			string errorText = await TryReadBusinessLogicErrorMessage(buffer, ct);
			error = ActionRecordError.FromNullableString(errorText);
		}

		ResetBufferToBeginning(buffer);
		await CopyClonedStreamToResponse(buffer, bodyClone, context, ct);
		ActionRecord action = ActionRecord.CreateNew(invokerId, name, payload, severity, error?.Value);
		PrintActionRecordInformation(action);
	}

	private static void ResetBufferToBeginning(Stream stream) => stream.Seek(0, SeekOrigin.Begin);

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
		using StreamReader reader = new(bufferFromResponse, Encoding.UTF8);
		string message = await reader.ReadToEndAsync(ct);
		ResetBufferToBeginning(bufferFromResponse);
		using JsonDocument document = JsonDocument.Parse(message);
		JsonElement messageField = document.RootElement.GetProperty("message");
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

	private void PrintActionRecordInformation(ActionRecord action)
	{
		Logger.Information(
			""" 
			Создана запись действия телеметрии:
			Идентификатор вызывающего объекта: {InvokerId}
			Имя действия: {Name}
			Уровень серьезности: {Severity}
			Данные действия (JSON): {PayloadJson}
			Дата и время возникновения действия: {OccuredDateTime}
			Ошибка: {Error}
			""",
			action.InvokerId is null ? "Аноним" : action.InvokerId.Value.Value,
			action.Name.Value,
			action.Severity.Value,
			action.PayloadJson is null ? "Отсутствуют" : action.PayloadJson.Value,
			action.OccuredDateTime.Value,
			action.Error is null ? "Отсутствует" : action.Error.Value
		);
	}

	// TODO: system errors do not have envelope, so give to payload infromation about system error.
	// TODO: business logic errors do have envelope, so give to payload information about business logic error (inspect response body json for that).
	private async Task<ActionRecordSeverity> InvokeAndReturnSeverity(HttpContext context)
	{
		try
		{
			await Next(context);
			return ResolveSeverityByStatusCode(context);
		}
		catch (Exception ex)
		{
			Logger.Error(ex, "Ошибка при создании записи действия телеметрии.");
			return ActionRecordSeverity.Error();
		}
	}
}
