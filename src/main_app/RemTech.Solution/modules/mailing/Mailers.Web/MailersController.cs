using System.Net;
using Mailers.Application.Features.ChangeMailerSmtpPassword;
using Mailers.Application.Features.CreateMailer;
using Mailers.Application.Features.DeleteMailer;
using Mailers.Application.Features.PingMailer;
using Mailers.Core.MailersModule;
using Mailers.Persistence.NpgSql.MailersModule;
using Microsoft.AspNetCore.Mvc;
using RemTech.Functional.Extensions;
using RemTech.Functional.Web.Extensions;

namespace Mailers.Web;

[ApiController]
[Route("api/mailing")]
public sealed class MailersController
{
    [HttpPost]
    public async Task<Envelope> RegisterNewMailer(
        [FromBody] RegisterNewMailerRequest request,
        [FromServices] CreateMailerUseCase useCase,
        CancellationToken ct)
    {
        CreateMailerArgs args = new(request.Email, request.SmtpPassword, ct);
        Result<Mailer> result = await useCase.Invoke(args);
        return result.AsEnvelope(() => new MailerResponse(result.Value));
    }

    [HttpDelete("{id:guid}")]
    public async Task<Envelope> DeleteMailerFromApplication(
        [FromRoute] Guid id,
        [FromServices] DeleteMailerUseCase useCase,
        CancellationToken ct)
    {
        DeleteMailerArgs args = new(id, ct);
        Result<Unit> result = await useCase.Invoke(args);
        return result.AsEnvelope(() => id);
    }

    [HttpPost("{id:guid}/ping")]
    public async Task<Envelope> PingMailer(
        [FromBody] PingMailerSenderRequest request,
        [FromRoute] Guid id,
        [FromServices] PingMailerUseCase useCase,
        CancellationToken ct)
    {
        PingMailerArgs args = new(id, request.To, ct);
        Result<Mailer> result = await useCase.Invoke(args);
        return result.AsEnvelope(() => new MailerResponse(result.Value));
    }

    [HttpPatch("{id:guid}/password")]
    public async Task<Envelope> ChangeMailerPassword(
        [FromBody] ChangeMailerSmtpPasswordRequest request,
        [FromRoute] Guid id,
        [FromServices] ChangeMailerSmtpPasswordUseCase useCase,
        CancellationToken ct)
    {
        ChangeMailerSmtpPasswordArgs args = new(id, request.NextPassword, ct);
        Result<Mailer> result = await useCase.Invoke(args);
        return result.AsEnvelope(() => new MailerResponse(result.Value));
    }
    
    [HttpGet()]
    public async Task<IResult> GetMailerSenders(
        [FromServices] NpgSqlMailersCommands commands,
        CancellationToken ct)
    {
        QueryMailerArguments args = new();
        IEnumerable<Mailer> mailers = await commands.GetMailers(args, ct);
        return Envelope.Map(mailers, m => new MailerResponse(m), HttpStatusCode.OK);
    }
}

public sealed record ChangeMailerSmtpPasswordRequest(string NextPassword);

public sealed record RegisterNewMailerRequest(string Email, string SmtpPassword);

public sealed record PingMailerSenderRequest(string To);

public sealed class MailerResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; }
    public int SendLimit { get; init; }
    public int SendCurrent { get; init; }

    public MailerResponse(Mailer mailer)
    {
        Id = mailer.Metadata.Id.Value;
        Email = mailer.Metadata.Email.Value;
        SendLimit = mailer.Statistics.SendCurrent;
        SendCurrent = mailer.Statistics.SendCurrent;
    }
}