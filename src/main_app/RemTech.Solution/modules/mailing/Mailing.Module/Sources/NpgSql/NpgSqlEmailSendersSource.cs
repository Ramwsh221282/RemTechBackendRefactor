using System.Data.Common;
using Mailing.Module.Contracts;
using Mailing.Module.Models;
using Npgsql;

namespace Mailing.Module.Sources.NpgSql;

internal sealed class NpgSqlEmailSendersSource(NpgsqlDataSource dataSource) : IEmailSendersSource
{
    public async Task<IEmailSender> Get(string email, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            SELECT name, email, key FROM mailing_module.senders
            WHERE email = @email
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@email", email));
        await command.PrepareAsync(ct);
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new InvalidOperationException($"Почтовый сервис {email} не существует.");
        return new EmailSender(
            reader.GetString(reader.GetOrdinal("email")),
            reader.GetString(reader.GetOrdinal("key"))
        );
    }

    public async Task<bool> Remove(IEmailSender sender, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            DELETE FROM mailing_module.senders
            WHERE name = @name;
            """
        );
        EmailSenderOutput output = sender.Print();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", output.Name));
        await command.PrepareAsync(ct);
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 1
            ? true
            : throw new InvalidOperationException($"Сервис с доменом {output.Name} не найден.");
    }

    public async Task<IEnumerable<IEmailSender>> ReadAll(CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            SELECT name, email, key FROM mailing_module.senders
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        List<IEmailSender> senders = [];
        while (await reader.ReadAsync(ct))
        {
            string email = reader.GetString(reader.GetOrdinal("email"));
            string key = reader.GetString(reader.GetOrdinal("key"));
            senders.Add(new EmailSender(email, key));
        }
        return senders;
    }

    public async Task<bool> Save(IEmailSender sender, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            INSERT INTO mailing_module.senders(name, email, key)
            VALUES (@name, @email, @key)
            ON CONFLICT(name)
            DO NOTHING;
            """
        );
        EmailSenderOutput output = sender.Print();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", output.Name));
        command.Parameters.Add(new NpgsqlParameter<string>("@email", output.Email));
        command.Parameters.Add(new NpgsqlParameter<string>("@key", output.Key));
        await command.PrepareAsync(ct);
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 1
            ? true
            : throw new InvalidOperationException($"Почтовый сервис: {output.Name} уже подключен.");
    }
}
