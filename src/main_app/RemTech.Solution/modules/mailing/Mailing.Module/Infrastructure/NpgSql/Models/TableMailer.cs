using Mailing.Module.Domain.Models;
using Mailing.Module.Domain.Models.ValueObjects;

namespace Mailing.Module.Infrastructure.NpgSql.Models;

internal sealed class TableMailer
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required int CurrentSent { get; init; }
    public required int CurrentLimit { get; init; }

    public IMailer ToMailer() => new Mailer(
        new Metadata(Id, Email, Password),
        new Statistics(CurrentLimit, CurrentSent));
}