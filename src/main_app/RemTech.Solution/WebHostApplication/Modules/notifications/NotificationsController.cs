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

namespace WebHostApplication.Modules.notifications;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : Controller
{
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
    
    [VerifyToken]
    [NotificationsManagementPermission]
    [HttpPost("mailers/{id:guid}/test-message")]
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