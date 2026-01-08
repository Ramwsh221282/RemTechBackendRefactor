using Microsoft.AspNetCore.Mvc;
using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Features.AddMailer;
using Notifications.Core.Mailers.Features.ChangeCredentials;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using WebHostApplication.ActionFilters.Attributes;

namespace WebHostApplication.Modules.notifications;

[ApiController]
[Route("api/notifications")]
public class NotificationsController
{
    [VerifyToken]
    [NotificationsManagementPermission]
    [HttpPost("mailer")]
    public async Task<Envelope> AddMailer(
        [FromBody] AddMailerRequest request,
        [FromServices] ICommandHandler<AddMailerCommand, Mailer> handler,
        CancellationToken ct
        )
    {
        AddMailerCommand command = new(request.SmtpPassword, request.Email);
        Result<Mailer> result = await handler.Execute(command, ct);
        return EnvelopedResultsExtensions.AsEnvelope(result, r => MailerResponse.ConvertFrom(r.Value));
    }

    [VerifyToken]
    [NotificationsManagementPermission]
    [HttpPut("mailer/{id:guid}")]
    public async Task<Envelope> UpdateMailer(
        [FromRoute(Name = "id")] Guid id,
        [FromBody] ChangeMailerRequest request,
        [FromServices] ICommandHandler<ChangeCredentialsCommand, Mailer> handler,
        CancellationToken ct
        )
    {
        ChangeCredentialsCommand command = new(id, request.SmtpPassword, request.Email);
        Result<Mailer> result = await handler.Execute(command, ct);
        return result.AsTypedEnvelope(MailerResponse.ConvertFrom);
    }
}