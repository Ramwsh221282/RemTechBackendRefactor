using Dapper;
using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using RemTech.NpgSql.Abstractions;

namespace Mailing.Infrastructure.NpgSql.Mailers;

// CREATE TABLE mailing_module.mailers
// (
//     id uuid primary key,
//     hashed_password varchar(256) NOT NULL,
//     service varchar(128) NOT NULL,
//     email varchar(256) NOT NULL,
//     send_limit INT NOT NULL,
//     send_current INT NOT NULL,
// );
public sealed record NpgSqlGetMailerProtocol(NpgSqlSession Session) : GetMailerProtocol
{
    public async Task<Mailer?> AvailableBySendLimit(CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM mailing_module.mailers WHERE send_current < send_limit";
        CommandDefinition command = new(sql, cancellationToken: ct, transaction: Session.Transaction);
        TableMailer? mailer = await Session.QueryMaybeRow<TableMailer>(command);
        return mailer?.ToMailer();
    }

    public async Task<Mailer?> ByEmail(string email, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM mailing_module.mailers WHERE email = @email";
        CommandDefinition command = new(sql, new { email },  cancellationToken: ct, transaction: Session.Transaction);
        TableMailer? mailer = await Session.QueryMaybeRow<TableMailer>(command);
        return mailer?.ToMailer();
    }

    public async Task<Mailer?> ById(Guid id, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM mailing_module.mailers WHERE @id = id";
        CommandDefinition command = new(sql, new { id }, cancellationToken: ct, transaction: Session.Transaction);
        TableMailer? mailer = await Session.QueryMaybeRow<TableMailer>(command);
        return mailer?.ToMailer();
    }
}

public sealed record TableMailer(
    Guid id, 
    string HashedPassword, 
    string Service, 
    string Email, 
    int SendLimit, 
    int SendCurrent)
{
    public Mailer ToMailer()
    {
        Core.Common.Email email = new(Email);
        MailerConfig config = new(HashedPassword);
        MailerDomain domain = new(email, Service, "", SendCurrent, SendLimit);
        return new Mailer(id, domain.WithResolvedService(), config);
    }
}