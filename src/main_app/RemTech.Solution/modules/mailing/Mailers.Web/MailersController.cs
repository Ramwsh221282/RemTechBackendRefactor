using Mailers.Application.Features;
using Mailers.Core.MailersContext;
using Mailers.Persistence.NpgSql;
using Microsoft.AspNetCore.Mvc;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Web;

[ApiController]
[Route("api/mailing")]
public sealed class MailersController
{
    [HttpPost]
    public async Task<IResult> RegisterNewMailer(
        [FromBody] RegisterNewMailerRequest request,
        [FromServices] IAsyncFunction<RegisterMailerInApplicationFunctionArgument, Result<Mailer>> function,
        [FromServices] NpgSqlSession dbSession,
        CancellationToken ct)
    {
        var argument = new RegisterMailerInApplicationFunctionArgument(request.Email, request.SmtpPassword, dbSession);
        var result = await function.Invoke(argument, ct);
        return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(new MailerResponse(result.Value));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeleteMailerFromApplication(
        [FromRoute] Guid id,
        [FromServices] IAsyncFunction<DeleteMailerFromApplicationFunctionArgument, Result<Mailer>> function,
        [FromServices] NpgSqlSession dbSession,
        CancellationToken ct)
    {
        var argument = new DeleteMailerFromApplicationFunctionArgument(id, dbSession);
        var result = await function.Invoke(argument, ct);
        return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(new MailerResponse(result.Value));
    }

    [HttpPost("{id:guid}/ping")]
    public async Task<IResult> PingMailer(
        [FromBody] PingMailerSenderRequest request,
        [FromRoute] Guid id,
        [FromServices] IAsyncFunction<PingMailerInApplicationFunctionArgument, Result<Mailer>> function,
        [FromServices] NpgSqlSession dbSession,
        CancellationToken ct)
    {
        var argument = new PingMailerInApplicationFunctionArgument(request.Id, request.To, dbSession);
        var result = await function.Invoke(argument, ct);
        return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(new MailerResponse(result.Value));
    }

    [HttpGet()]
    public async Task<IResult> GetMailerSenders(
        [FromServices] NpgSqlSession dbSession,
        CancellationToken ct)
    {
        var mailers = await new QueryMailerArguments().GetMany(dbSession, ct);
        return Results.Ok(mailers.Select(m => new MailerResponse(m)));
    }
}

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