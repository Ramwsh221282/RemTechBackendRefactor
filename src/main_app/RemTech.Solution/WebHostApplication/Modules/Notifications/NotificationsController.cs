using System.Net;
using Microsoft.AspNetCore.Mvc;
using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Features.AddMailer;
using Notifications.Core.Mailers.Features.ChangeCredentials;
using Notifications.Core.Mailers.Features.DeleteMailer;
using Notifications.Core.PendingEmails.Features.AddPendingEmail;
using Notifications.Infrastructure.Mailers.Queries.GetMailer;
using Notifications.Infrastructure.Mailers.Queries.GetMailers;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using WebHostApplication.ActionFilters.Attributes;
using WebHostApplication.Common.Envelope;

namespace WebHostApplication.Modules.Notifications;

/// <summary>
/// Контроллер для работы с уведомлениями.
/// </summary>
[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
	/// <summary>
	/// Получить список конфигураций почтовых сервисов.
	/// </summary>
	/// <param name="handler">Обработчик запроса на получение списка почтовых сервисов.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом выполнения запроса.</returns>
	[VerifyToken]
	[NotificationsManagementPermission]
	[HttpGet("mailers")]
	public async Task<Envelope> GetMailers(
		[FromServices] IQueryHandler<GetMailersQuery, IEnumerable<MailerResponse>> handler,
		CancellationToken ct
	)
	{
		GetMailersQuery query = new();
		IEnumerable<MailerResponse> result = await handler.Handle(query, ct);
		return new Envelope((int)HttpStatusCode.OK, result, null);
	}

	/// <summary>
	/// Удалить конфигурацию почтового сервиса.
	/// </summary>
	/// <param name="handler">Обработчик команды на удаление конфигурации почтового сервиса.</param>
	/// <param name="id">Идентификатор конфигурации почтового сервиса.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом выполнения команды.</returns>
	[VerifyToken]
	[NotificationsManagementPermission]
	[HttpDelete("mailers/{id:guid}")]
	public async Task<Envelope> DeleteMailer(
		[FromServices] ICommandHandler<DeleteMailerCommand, Guid> handler,
		[FromRoute(Name = "id")] Guid id,
		CancellationToken ct
	)
	{
		DeleteMailerCommand command = new(id);
		Result<Guid> result = await handler.Execute(command, ct);
		return result.AsEnvelope();
	}

	/// <summary>
	/// Отправить тестовое сообщение с указанного почтового сервиса.
	/// </summary>
	/// <param name="handler">Обработчик команды на добавление отложенного письма.</param>
	/// <param name="request">Запрос с данными для отправки тестового сообщения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом выполнения команды.</returns>
	[VerifyToken]
	[NotificationsManagementPermission]
	[HttpPost("mailers/test-message")]
	public async Task<Envelope> SendTestMessage(
		[FromServices] ICommandHandler<AddPendingEmailCommand, Unit> handler,
		[FromBody] SendTestMessageRequest request,
		CancellationToken ct
	)
	{
		AddPendingEmailCommand command = new(request.Recipient, "Тестовое сообщение", "Тестовая отправка сообщения.");
		Result<Unit> result = await handler.Execute(command, ct);
		return EnvelopedResultsExtensions.AsEnvelope(result);
	}

	/// <summary>
	/// Получить конфигурацию почтового сервиса по идентификатору.
	/// </summary>
	/// <param name="id">Идентификатор конфигурации почтового сервиса.</param>
	/// <param name="handler">Обработчик запроса на получение конфигурации почтового сервиса.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом выполнения запроса.</returns>
	[VerifyToken]
	[NotificationsManagementPermission]
	[HttpGet("mailers/{id:guid}")]
	public async Task<Envelope> GetMailer(
		[FromRoute(Name = "id")] Guid id,
		[FromServices] IQueryHandler<GetMailerQuery, MailerResponse?> handler,
		CancellationToken ct
	)
	{
		GetMailerQuery query = new(id);
		MailerResponse? mailer = await handler.Handle(query, ct);
		return mailer.NotFoundOrOk("Не найдена конфигурация почтового сервиса.");
	}

	/// <summary>
	/// Добавить конфигурацию почтового сервиса.
	/// </summary>
	/// <param name="request">Запрос с данными для добавления конфигурации почтового сервиса.</param>
	/// <param name="handler">Обработчик команды на добавление конфигурации почтового сервиса.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом выполнения команды.</returns>
	[VerifyToken]
	[NotificationsManagementPermission]
	[HttpPost("mailers")]
	public async Task<Envelope> AddMailer(
		[FromBody] AddMailerRequest request,
		[FromServices] ICommandHandler<AddMailerCommand, Mailer> handler,
		CancellationToken ct
	)
	{
		AddMailerCommand command = new(request.SmtpPassword, request.Email);
		Result<Mailer> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(MailerResponse.Create);
	}

	/// <summary>
	/// Изменить конфигурацию почтового сервиса.
	/// </summary>
	/// <param name="id">Идентификатор конфигурации почтового сервиса.</param>
	/// <param name="request">Запрос с данными для изменения конфигурации почтового сервиса.</param>
	/// <param name="handler">Обработчик команды на изменение конфигурации почтового сервиса.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом выполнения команды.</returns>
	[VerifyToken]
	[NotificationsManagementPermission]
	[HttpPut("mailers/{id:guid}")]
	public async Task<Envelope> UpdateMailer(
		[FromRoute(Name = "id")] Guid id,
		[FromBody] ChangeMailerRequest request,
		[FromServices] ICommandHandler<ChangeCredentialsCommand, Mailer> handler,
		CancellationToken ct
	)
	{
		ChangeCredentialsCommand command = new(id, request.SmtpPassword, request.Email);
		Result<Mailer> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(MailerResponse.Create);
	}
}
