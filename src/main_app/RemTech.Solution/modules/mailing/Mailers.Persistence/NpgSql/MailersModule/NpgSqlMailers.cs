using System.Data;
using Dapper;
using Mailers.Core.MailersModule;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Persistence.NpgSql.MailersModule;

public static class NpgSqlMailers
{
    public static InsertMailer InsertMailer(NpgSqlSession session) => async (mailer, ct) =>
        {
            const string sql =
                """
                INSERT INTO mailers_module.mailers(id, email, smtp_password, send_limit, send_at_this_moment)
                VALUES(@id, @email, @smtp_password, @send_limit, @send_at_this_moment)
                """;
            await session.Execute(mailer.CreateDatabaseCommand(sql, session, ct));
            return Unit.Value;
        };

    public static DeleteMailer DeleteMailer(NpgSqlSession session) => async (mailer, ct) =>
        {
            const string sql = "DELETE FROM mailers_module.mailers WHERE id = @id";
            int affected = await session.CountAffected(mailer.CreateDatabaseCommand(sql, session, ct));
            return affected == 0 ? Conflict("Почтовый отправитель не найден.") : Unit.Value;
        };

    public static HasUniqueMailerEmail HasUniqueMailerEmail(NpgSqlSession session) => async (email, ct) =>
        {
            const string sql = "SELECT EXISTS(SELECT 1 FROM mailers_module.mailers WHERE email = @email);";
            DynamicParameters parameters = new();
            parameters.Add("@email", email.Value, DbType.String);
            CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
            bool exists = await session.QuerySingleRow<bool>(command);
            return !exists;
        };

    public static GetMailer GetMailer(NpgSqlSession session) => (args, ct) => args.Get(session, ct);

    public static GetManyMailers GetMany(NpgSqlSession session) => (args, ct) => args.GetMany(session, ct);

    public static UpdateMailer UpdateMailer(NpgSqlSession session) => async (mailer, ct) =>
    {
        const string sql =
            """
            UPDATE 
                mailers_module.mailers
            SET
                email = @email,
                smtp_password = @smtp_password,
                send_limit = @send_limit,
                send_at_this_moment = @send_at_this_moment
            WHERE 
                id = @id;
            """;
        await session.Execute(mailer.CreateDatabaseCommand(sql, session, ct));
        return Unit.Value;
    };
    
    extension(Mailer mailer)
    {
        private CommandDefinition CreateDatabaseCommand(string sql, NpgSqlSession session, CancellationToken ct) =>
            new(sql, 
                mailer.FillParameters(), 
                cancellationToken: ct,
                transaction: session.Transaction);
        
        private DynamicParameters FillParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id", mailer.Metadata.Id.Value, DbType.Guid);
            parameters.Add("@email", mailer.Metadata.Email.Value, DbType.String);
            parameters.Add("@smtp_password", mailer.Password.Value, DbType.String);
            parameters.Add("@send_limit", mailer.Statistics.Limit, DbType.Int32);
            parameters.Add("@send_at_this_moment", mailer.Statistics.SendCurrent, DbType.Int32);
            return parameters;
        }
    }

    extension(QueryMailerArguments args)
    {
        private async Task<IEnumerable<Mailer>> GetMany(NpgSqlSession session, CancellationToken ct = default)
        {
            string clause = args.WhereClause(args.ParameterClauses());
            string sql = $"SELECT * FROM mailers_module.mailers {clause}";
            IEnumerable<TableMailer> collection = await session.QueryMultipleRows<TableMailer>(args.CreateCommand(sql, session, ct));
            IEnumerable<Mailer> mailers = collection.Select(c => c.ToMailer());
            return mailers;
        }
        
        private async Task<Optional<Mailer>> Get(NpgSqlSession session, CancellationToken ct = default)
        {
            string clause = args.WhereClause(args.ParameterClauses());
            if (string.IsNullOrEmpty(clause)) return None<Mailer>();
            string sql = $"SELECT * FROM mailers_module.mailers {clause} LIMIT 1 {args.LockClause(args.WithLock)}";
            var fromDb = await session.QueryMaybeRow<TableMailer?>(args.CreateCommand(sql, session, ct));
            return fromDb == null ? None<Mailer>() : Some(fromDb.ToMailer());
        }

        private string LockClause(bool isLocking)
        {
            return isLocking ? " FOR UPDATE " : string.Empty;
        }
        
        private string WhereClause(string[] clauses)
        {
            return clauses.All(String.EmptyOrWhiteSpace) ? string.Empty : "WHERE " + string.Join(" AND ", clauses);
        }
        
        private string[] ParameterClauses()
        {
            string[] clauses =
            [
                !string.IsNullOrWhiteSpace(args.Email) ? "email = @email" : string.Empty,
                !string.IsNullOrWhiteSpace(args.SmtpPassword) ? "smtp_password = @smtp_password" : string.Empty,
                args.Id.HasValue ? $"id = @id" : string.Empty,
                args.SendLimit.HasValue ? $"limit = @send_limit" : string.Empty,
                args.SendCurrent.HasValue ? $"current = @send_at_this_moment" : string.Empty,
            ];
            return clauses.Where(c => !string.IsNullOrEmpty(c)).ToArray();
        }

        private DynamicParameters FillParameters()
        {
            DynamicParameters parameters = new();
            if (args.Id.HasValue) parameters.Add("@id", args.Id.Value, DbType.Guid);
            if (!string.IsNullOrWhiteSpace(args.Email)) parameters.Add("@email", args.Email, DbType.String);
            if (!string.IsNullOrWhiteSpace(args.SmtpPassword)) parameters.Add("@smtp_password", args.SmtpPassword, DbType.String);
            if (args.SendLimit.HasValue) parameters.Add("@send_limit", args.SendLimit.Value, DbType.Int32);
            if (args.SendCurrent.HasValue) parameters.Add("@send_at_this_moment", args.SendCurrent.Value, DbType.Int32);
            return parameters;
        }
        
        private CommandDefinition CreateCommand(string sql, NpgSqlSession session, CancellationToken ct) =>
            new(sql, args.FillParameters(), cancellationToken: ct, transaction: session.Transaction);
    }
}