using Mailing.Module.Infrastructure.NpgSql;
using Mailing.Module.MailersContext.Factories;
using Mailing.Module.MailersContext.MetadataContext.Factories;
using Mailing.Module.MailersContext.StatisticsContext.Factories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Module.MailersContext.Controllers;

public sealed record CreateMailerRequest(string Email, string Password);

[ApiController]
[Route("api/mailers")]
public sealed class MailersController : Controller
{
    [HttpPost]
    public async Task<IResult> Create(
        [FromBody] CreateMailerRequest request,
        [FromServices] PostgresDatabase db,
        CancellationToken ct)
    {
        var factory = new MailersFactory(new MailersMetadataFactory(), new MailersStatisticsFactory());
        var metadata = new MailersMetadataFactory.Input(Guid.Empty, request.Email, request.Password);
        var statistics = new MailersStatisticsFactory.Input(0, 0);
        var mailer = factory.Create(new MailersFactory.Input(metadata, statistics));
        await using var persistence = new PgPersistence(await db.DataSource.OpenConnectionAsync(ct));
        await mailer.Persist(persistence, ct);
        return Results.Ok(mailer.Snapshotted());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IResult> Delete(
        [FromRoute] Guid id,
        [FromServices] PostgresDatabase db,
        CancellationToken ct
    )
    {
        await using var persistence = new PgPersistence(await db.DataSource.OpenConnectionAsync(ct));
        var mailerStatus = await persistence.Find(new PgMailerByIdSearch(id), ct);
        if (mailerStatus.IsFailure)
            return Results.BadRequest(mailerStatus.Error);
        await mailerStatus.Value.Remove(persistence, ct);
        return Results.Ok();
    }
}