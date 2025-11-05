using System.Data;
using Dapper;
using Mailing.Domain.Postmans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mailing.Adapters.Storage.Postmans;

/* Mailing module postmans database schema:
// CREATE TABLE mailing_module.postmans
// (
//     id       UUID PRIMARY KEY,
//     email    varchar(255) not null UNIQUE,
//     password varchar(512) not null
// );
*/
internal sealed class DbPostman(IDbConnection connection, IPostman postman) : PostmanEnvelope(postman)
{
    private DynamicParameters? _parameters;

    private DynamicParameters Parameters
    {
        get
        {
            if (_parameters != null)
                return _parameters;
            _parameters = new DynamicParameters();
            _parameters.Add("@email", Data.Email, DbType.String);
            _parameters.Add("@password", Data.SmtpPassword, DbType.String);
            _parameters.Add("@id", Data.Id, DbType.Guid);
            return _parameters;
        }
    }

    public async Task Delete(CancellationToken ct) =>
        await connection.ExecuteAsync(new CommandDefinition(
            """
            DELETE FROM mailing_module.postmans 
            WHERE id = @id
            """,
            Parameters,
            cancellationToken: ct));

    public async Task Update(CancellationToken ct) =>
        await connection.ExecuteAsync(new CommandDefinition(
            """
            UPDATE mailing_module.postmans
            SET email = @email,
                password = @password
                WHERE id = @id
            """,
            Parameters,
            cancellationToken: ct
        ));

    public async Task Save(CancellationToken ct) =>
        await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO mailing_module.postmans
            (id, email, password)
            VALUES
            (@id, @email, @password)
            """,
            Parameters,
            cancellationToken: ct));

    public async Task<bool> HasUniqueEmail(CancellationToken ct) =>
        !(await connection.QuerySingleAsync<bool>(
            new CommandDefinition(
                """
                SELECT 
                    EXISTS(SELECT 1 FROM mailing_module.postmans 
                WHERE 
                    email = @email);
                """,
                Parameters,
                cancellationToken: ct)));
}