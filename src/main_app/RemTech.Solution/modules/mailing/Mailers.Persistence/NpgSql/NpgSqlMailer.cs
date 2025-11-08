using System.Data;
using Dapper;
using Npgsql;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Persistence.NpgSql;

/*
 * TABLE SCHEMA:
 * id: UUID KEY,
 * email: VARCHAR(256) NOT NULL,
 * smtp_password: VARCHAR(512) NOT NULL,
 * send_limit: INT NOT NULL,
 * send_at_this_moment: INT NOT NULL
 */
public sealed class NpgSqlMailer(NpgSqlConnectionFactory factory) : IAsyncDisposable
{
    private readonly NpgSqlConnectionFactory _factory = factory;
    private Optional<Guid> _id = None<Guid>();
    private Optional<string> _email = None<string>();
    private Optional<string> _smtpPassword = None<string>();
    private Optional<int> _sendLimit = None<int>();
    private Optional<int> _sendAtThisMoment = None<int>();
    private Optional<NpgsqlConnection> _connection = None<NpgsqlConnection>();
    private Optional<NpgsqlTransaction> _transaction = None<NpgsqlTransaction>();

    public NpgSqlMailer WithId(Optional<Guid> id)
    {
        _id = id;
        return this;
    }

    public NpgSqlMailer WithEmail(Optional<string> email)
    {
        _email = email;
        return this;
    }

    public NpgSqlMailer WithSmtpPassword(Optional<string> smtpPassword)
    {
        _smtpPassword = smtpPassword;
        return this;
    }

    public NpgSqlMailer WithSendLimit(Optional<int> sendLimit)
    {
        _sendLimit = sendLimit;
        return this;
    }

    public NpgSqlMailer WithSendAtThisMoment(Optional<int> sendAtThisMoment)
    {
        _sendAtThisMoment = sendAtThisMoment;
        return this;
    }

    public async Task<bool> IsEmailUnique(CancellationToken ct = default)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM mailers_module.mailers WHERE email = @email);";
        var connection = await ExposeConnection(ct);
        var exists = await connection.QuerySingleAsync<bool>(
            new CommandDefinition(
                sql,
                FillParameters(),
                cancellationToken: ct)); 
        return !exists;
    }
    
    public async Task Save(CancellationToken ct = default)
    {
        const string sql =
            """
            INSERT INTO mailers_module.mailers(id, email, smtp_password, send_limit, send_at_this_moment)
            VALUES(@id, @email, @smtp_password, @send_limit, @send_at_this_moment)
            """;
        var connection = await ExposeConnection(ct);
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                FillParameters(),
                cancellationToken: ct));
    }

    public async Task Update(CancellationToken ct = default)
    {
        string[] parameters = ParameterClauses();
        string updateClauses = UpdateClause(parameters);
        if (String.EmptyOrWhiteSpace(updateClauses))
            throw new InvalidOperationException("Нет update clauses при обновлении Mailer.");
        string sql = $"UPDATE mailers_module.mailers {updateClauses} WHERE  id = @id";
        await (await ExposeConnection(ct)).ExecuteAsync(new CommandDefinition(
            sql,
            FillParameters(),
            cancellationToken: ct
        ));
    }

    public async Task<Optional<NpgSqlMailer>> FindSingle(CancellationToken ct = default)
    {
        string[] parameters = ParameterClauses();
        string whereClause = WhereClause(parameters);
        if (String.EmptyOrWhiteSpace(whereClause))
            return None<NpgSqlMailer>();
        string sql = $"SELECT * FROM mailers_module.mailers {whereClause}";
        TableMailer? mailer = await (await ExposeConnection(ct))
            .QueryFirstOrDefaultAsync<TableMailer>(new CommandDefinition(
                sql,
                FillParameters(),
                cancellationToken: ct));
        return FromTableMailer(this, mailer);
    }

    public async Task Delete(CancellationToken ct = default)
    {
        const string sql = "DELETE FROM mailers_module.mailers WHERE id = @id";
        var connection = await ExposeConnection(ct);
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                FillParameters(),
                cancellationToken: ct));
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.ExecuteOnValueAsync(async conn => await conn.DisposeAsync());
    }
    
    private string UpdateClause(string[] clauses)
    {
        return clauses.All(String.EmptyOrWhiteSpace) ? string.Empty : "SET " + string.Join(", ", clauses);
    }

    private string WhereClause(string[] clauses)
    {
        return clauses.All(String.EmptyOrWhiteSpace) ? string.Empty : "WHERE " + string.Join(" AND ", clauses);
    }

    private string[] ParameterClauses()
    {
        string[] clauses =
        [
            _email.Map("email = @email", string.Empty),
            _sendLimit.Map("send_limit = @send_limit", string.Empty),
            _sendAtThisMoment.Map("send_at_this_moment = @send_at_this_moment", string.Empty),
            _smtpPassword.Map("smtp_password = @smtp_password", string.Empty)
        ];
        return clauses;
    }

    private async Task<NpgsqlConnection> ExposeConnection(CancellationToken ct = default)
    {
        if (_connection.HasValue) return _connection.Value;
        var connection = await _factory.Create(ct);
        _connection = Some(connection);
        return connection;
    }

    private static Optional<NpgSqlMailer> FromTableMailer(NpgSqlMailer origin, TableMailer? mailer)
    {
        if (mailer is null) return None<NpgSqlMailer>();
        NpgSqlMailer @new = new NpgSqlMailer(origin._factory);
        @new._id = Some(mailer.Id);
        @new._email = Some(mailer.Email);
        @new._smtpPassword = Some(mailer.SmtpPassword);
        @new._sendLimit = Some(mailer.SendLimit);
        @new._sendAtThisMoment = Some(mailer.SendAtThisMoment);
        return Some(@new);
    }

    private DynamicParameters FillParameters()
    {
        var parameters = new DynamicParameters();
        if (_id.HasValue) parameters.Add("@id", _id.Value, DbType.Guid);
        if (_email.HasValue) parameters.Add("@email", _email.Value, DbType.String);
        if (_smtpPassword.HasValue) parameters.Add("@smtp_password", _email.Value, DbType.String);
        if (_sendLimit.HasValue) parameters.Add("@send_limit", _sendLimit.Value, DbType.Int32);
        if (_sendAtThisMoment.HasValue) parameters.Add("@send_at_this_moment", _sendAtThisMoment.Value, DbType.Int32);
        return parameters;
    }
}