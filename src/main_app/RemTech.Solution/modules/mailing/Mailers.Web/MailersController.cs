using System.Net;
using Mailers.Application.Features;
using Mailers.Core.MailersContext;
using Mailers.Persistence.NpgSql;
using Microsoft.AspNetCore.Mvc;
using RemTech.Functional.Extensions;
using RemTech.Functional.Web.Extensions;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Web;

[ApiController]
[Route("api/mailing")]
public sealed class MailersController
{
    [HttpPost]
    public async Task<Envelope> RegisterNewMailer(
        [FromBody] RegisterNewMailerRequest request,
        [FromServices] IAsyncFunction<RegisterMailerInApplicationFunctionArgument, Result<Mailer>> function,
        [FromServices] NpgSqlSession dbSession,
        CancellationToken ct)
    {
        RegisterMailerInApplicationFunctionArgument argument = new(request.Email, request.SmtpPassword, dbSession);
        Result<Mailer> result = await function.Invoke(argument, ct);
        return result.AsEnvelope(() => new MailerResponse(result.Value));
    }

    [HttpDelete("{id:guid}")]
    public async Task<Envelope> DeleteMailerFromApplication(
        [FromRoute] Guid id,
        [FromServices] IAsyncFunction<DeleteMailerFromApplicationFunctionArgument, Result<Mailer>> function,
        [FromServices] NpgSqlSession dbSession,
        CancellationToken ct)
    {
        DeleteMailerFromApplicationFunctionArgument argument = new(id, dbSession);
        Result<Mailer> result = await function.Invoke(argument, ct);
        return result.AsEnvelope(() => new MailerResponse(result.Value));
    }

    [HttpPost("{id:guid}/ping")]
    public async Task<Envelope> PingMailer(
        [FromBody] PingMailerSenderRequest request,
        [FromRoute] Guid id,
        [FromServices] IAsyncFunction<PingMailerInApplicationFunctionArgument, Result<Mailer>> function,
        [FromServices] NpgSqlSession dbSession,
        CancellationToken ct)
    {
        PingMailerInApplicationFunctionArgument argument = new(request.Id, request.To, dbSession);
        Result<Mailer> result = await function.Invoke(argument, ct);
        return result.AsEnvelope(() => new MailerResponse(result.Value));
    }

    [HttpPatch("{id:guid}/password")]
    public async Task<Envelope> ChangeMailerPassword(
        [FromBody] ChangeMailerSmtpPasswordRequest request,
        [FromRoute] Guid id,
        [FromServices] NpgSqlSession dbSession,
        [FromServices] IAsyncFunction<ChangeMailerSmtpPasswordFunctionArguments, Result<Mailer>> function,
        CancellationToken ct)
    {
        ChangeMailerSmtpPasswordFunctionArguments args = new(id, request.NextPassword, dbSession);
        Result<Mailer> result = await function.Invoke(args, ct);
        return result.AsEnvelope(() => new MailerResponse(result.Value));
    }
    
    [HttpGet()]
    public async Task<IResult> GetMailerSenders(
        [FromServices] NpgSqlSession dbSession,
        CancellationToken ct)
    {
        QueryMailerArguments args = new();
        IEnumerable<Mailer> mailers = await args.GetMany(dbSession, ct);
        return Envelope.Map(mailers, m => new MailerResponse(m), HttpStatusCode.OK);
    }
}

public sealed record ChangeMailerSmtpPasswordRequest(string NextPassword);

public sealed record RegisterNewMailerRequest(string Email, string SmtpPassword);

public sealed record PingMailerSenderRequest(Guid Id, string To);

public sealed class MailerResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; }
    public int SendLimit { get; init; }
    public int SendCurrent { get; init; }

    public MailerResponse(Mailer mailer)
    {
        Id = mailer.Metadata.Id;
        Email = mailer.Metadata.Email.Value;
        SendLimit = mailer.Statistics.SendCurrent;
        SendCurrent = mailer.Statistics.SendCurrent;
    }
}