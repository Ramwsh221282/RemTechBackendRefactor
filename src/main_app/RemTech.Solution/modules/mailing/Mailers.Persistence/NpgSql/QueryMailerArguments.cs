namespace Mailers.Persistence.NpgSql;

/*
 * TABLE SCHEMA:
 * id: UUID KEY,
 * email: VARCHAR(256) NOT NULL,
 * smtp_password: VARCHAR(512) NOT NULL,
 * send_limit: INT NOT NULL,
 * send_at_this_moment: INT NOT NULL
 */
public sealed record QueryMailerArguments(
    Guid? Id = null, 
    string? Email = null, 
    string? SmtpPassword = null, 
    int? SendLimit = null, 
    int? SendCurrent = null);