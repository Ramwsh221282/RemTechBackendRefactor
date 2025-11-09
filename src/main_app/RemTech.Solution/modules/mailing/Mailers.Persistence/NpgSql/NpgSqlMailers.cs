using System.Data;
using Dapper;
using Mailers.Core.MailersContext;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Persistence.NpgSql;

public static class NpgSqlMailers
{
    extension(Mailer mailer)
    {
        public async Task<bool> HasUniqueEmail(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql = "SELECT EXISTS(SELECT 1 FROM mailers_module.mailers WHERE email = @email);";
            bool exists = await session.QuerySingleRow<bool>(mailer.CreateDatabaseCommand(sql, session, ct));
            return !exists;
        }
        
        public async Task Insert(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql =
                """
                INSERT INTO mailers_module.mailers(id, email, smtp_password, send_limit, send_at_this_moment)
                VALUES(@id, @email, @smtp_password, @send_limit, @send_at_this_moment)
                """;
            await session.Execute(mailer.CreateDatabaseCommand(sql, session, ct));
        }

        public async Task<Result<Unit>> Delete(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql = "DELETE FROM mailers_module.mailers WHERE id = @id";
            var affected = await session.CountAffected(mailer.CreateDatabaseCommand(sql, session, ct));
            return affected == 0 ? Conflict("Почтовый отправитель не найден.") : Unit.Value;
        }

        public async Task Update(NpgSqlSession session, CancellationToken ct = default)
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
        }
        
        private string WhereClause(string[] clauses)
        {
            return clauses.All(String.EmptyOrWhiteSpace) ? string.Empty : "WHERE " + string.Join(" AND ", clauses);
        }
        
        
        private CommandDefinition CreateDatabaseCommand(string sql, NpgSqlSession session, CancellationToken ct) =>
            new(sql, 
                mailer.FillParameters(), 
                cancellationToken: ct,
                transaction: session.Transaction);
        
        private DynamicParameters FillParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id", mailer.Metadata.Id, DbType.Guid);
            parameters.Add("@email", mailer.Metadata.Email.Value, DbType.String);
            parameters.Add("@smtp_password", mailer.Metadata.Password.Value, DbType.String);
            parameters.Add("@send_limit", mailer.Statistics.Limit, DbType.Int32);
            parameters.Add("@send_at_this_moment", mailer.Statistics.SendCurrent, DbType.Int32);
            return parameters;
        }
    }

    extension(QueryMailerArguments args)
    {
        public async Task<IEnumerable<Mailer>> GetMany(NpgSqlSession session, CancellationToken ct = default)
        {
            var clause = args.WhereClause(args.ParameterClauses());
            var sql = $"SELECT * FROM mailers_module.mailers {clause}";
            var collection = await session.QueryMultipleRows<TableMailer>(args.CreateCommand(sql, session, ct));
            var mailers = collection.Select(c => c.ToMailer());
            return mailers;
        }
        
        public async Task<Optional<Mailer>> Get(
            NpgSqlSession session,
            CancellationToken ct = default,
            bool withLock = false)
        {
            var clause = args.WhereClause(args.ParameterClauses());
            if (string.IsNullOrEmpty(clause)) return None<Mailer>();
            var sql = $"SELECT * FROM mailers_module.mailers {clause} LIMIT 1 {args.LockClause(withLock)}";
            var fromDb = await session.QueryMaybeRow<TableMailer?>(args.CreateCommand(sql, session, ct));
            if (fromDb == null) return None<Mailer>();
            return Some(fromDb.ToMailer());
        }

        private string LockClause(bool isLocking)
        {
            return isLocking ? " FOR UPDATE " : string.Empty;
        }
        
        private string WhereClause(string[] clauses)
        {
            return clauses.All(String.EmptyOrWhiteSpace) ? string.Empty : "WHERE " + string.Join(" AND ", clauses);
        }
        
        internal string[] ParameterClauses()
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

        internal DynamicParameters FillParameters()
        {
            var parameters = new DynamicParameters();
            if (args.Id.HasValue) parameters.Add("@id", args.Id.Value, DbType.Guid);
            if (!string.IsNullOrWhiteSpace(args.Email)) parameters.Add("@email", args.Email, DbType.String);
            if (!string.IsNullOrWhiteSpace(args.SmtpPassword)) parameters.Add("@smtp_password", args.SmtpPassword, DbType.String);
            if (args.SendLimit.HasValue) parameters.Add("@send_limit", args.SendLimit, DbType.Int32);
            if (args.SendCurrent.HasValue) parameters.Add("@send_at_this_moment", args.SendCurrent, DbType.Int32);
            return parameters;
        }
        
        internal CommandDefinition CreateCommand(string sql, NpgSqlSession session, CancellationToken ct) =>
            new(sql, 
                args.FillParameters(),
                cancellationToken: ct,
                transaction: session.Transaction);
    }
}