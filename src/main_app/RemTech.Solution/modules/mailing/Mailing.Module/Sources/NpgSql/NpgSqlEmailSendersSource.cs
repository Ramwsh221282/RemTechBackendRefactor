using System.Data.Common;
using Mailing.Module.Configuration;
using Mailing.Module.Contracts;
using Mailing.Module.Models;
using Npgsql;

namespace Mailing.Module.Sources.NpgSql;

internal sealed class NpgSqlEmailSendersSource(MailingModuleOptions options)
    : IDisposable,
        IAsyncDisposable,
        IEmailSendersSource
{
    private readonly NpgsqlDataSource _source = new NpgsqlDataSourceBuilder(
        options.Database.ToConnectionString()
    ).Build();

    public void Dispose() => _source.Dispose();

    public async ValueTask DisposeAsync() => await _source.DisposeAsync();

    public async Task<IEmailSender> Get(string name, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            SELECT name, email, key FROM mailing_module.senders
            WHERE name = @name
            """
        );
        await using NpgsqlConnection connection = await _source.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", name));
        await command.PrepareAsync(ct);
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new InvalidOperationException($"Почтовый сервис {name} не существует.");
        return new EmailSender(
            reader.GetString(reader.GetOrdinal("name")),
            reader.GetString(reader.GetOrdinal("email")),
            reader.GetString(reader.GetOrdinal("key"))
        );
    }

    public async Task<bool> Update(IEmailSender sender, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            UPDATE mailing_module.senders
            SET email = @email, key = @key
            ON CONFLICT(email) DO NOTHING;
            """
        );
        EmailSenderOutput output = sender.Print();
        await using NpgsqlConnection connection = await _source.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@email", output.Email));
        command.Parameters.Add(new NpgsqlParameter<string>("@key", output.Key));
        await command.PrepareAsync(ct);
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 1
            ? true
            : throw new InvalidOperationException($"Сервис с почтой {output.Email} уже занят.");
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
        await using NpgsqlConnection connection = await _source.OpenConnectionAsync(ct);
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
        await using NpgsqlConnection connection = await _source.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        List<IEmailSender> senders = [];
        while (await reader.ReadAsync(ct))
        {
            string name = reader.GetString(reader.GetOrdinal("name"));
            string email = reader.GetString(reader.GetOrdinal("email"));
            string key = reader.GetString(reader.GetOrdinal("key"));
            senders.Add(new EmailSender(name, email, key));
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
        await using NpgsqlConnection connection = await _source.OpenConnectionAsync(ct);
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
