using System.Data;
using Dapper;
using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using RemTech.SharedKernel.Infrastructure.NpgSql;

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
public sealed class NpgSqlGetMailerProtocol(NpgSqlSession session) : GetMailerProtocol
{
    public async Task<Mailer?> AvailableBySendLimit(bool withLock = false, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM mailing_module.mailers WHERE send_current < send_limit";
        CommandDefinition command = new(sql, cancellationToken: ct, transaction: session.Transaction);
        TableMailer? mailer = await session.QueryMaybeRow<TableMailer>(command);
        return mailer?.ToMailer();
    }

    public async Task<Mailer?> Get(GetMailerQueryArgs args, CancellationToken ct = default)
    {
        MailerQueryOptions options = new(args);
        string sql = $"SELECT * FROM mailing_module.mailers {options.WhereClause} {options.LockClause}";
        CommandDefinition command = options.CreateCommand(sql, session, ct);
        TableMailer? mailer = await session.QueryMaybeRow<TableMailer>(command);
        return mailer?.ToMailer();
    }
    
    private sealed record TableMailer(
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
    
    private sealed class MailerQueryOptions
    {
        private readonly DynamicParameters _parameters = new();
        private readonly List<string> _filters = [];
        public string LockClause { get; private set; } = string.Empty;
        public string WhereClause => _filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", _filters);

        public CommandDefinition CreateCommand(string sql, NpgSqlSession session, CancellationToken ct)
        {
            CommandDefinition command = new(sql, _parameters, cancellationToken: ct, transaction: session.Transaction);
            return command;
        }
    
        public MailerQueryOptions(GetMailerQueryArgs args)
        {
            if (args.WithLock) LockClause = "FOR UPDATE";
            if (args.Id.HasValue)
            {
                _filters.Add("id = @id");
                _parameters.Add("@id", args.Id.Value, DbType.Guid);
            }

            if (!string.IsNullOrWhiteSpace(args.Email))
            {
                _filters.Add("@email = email");
                _parameters.Add("@email", args.Email, DbType.String);
            }
        }
    }
}

