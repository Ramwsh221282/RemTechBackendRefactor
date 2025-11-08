namespace Mailers.Persistence.NpgSql;

/*
 * TABLE SCHEMA:
 * id: UUID KEY,
 * email: VARCHAR(256) NOT NULL,
 * smtp_password: VARCHAR(512) NOT NULL,
 * send_limit: INT NOT NULL,
 * send_at_this_moment: INT NOT NULL
 */
internal sealed class TableMailer
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string SmtpPassword { get; init; }
    public required int SendLimit { get; init; }
    public required int SendAtThisMoment { get; init; }
}