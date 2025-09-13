using Mailing.Module.Bus;
using Npgsql;
using Users.Module.CommonAbstractions;
using Users.Module.Features.CreateEmailConfirmation;

namespace Users.Module.Features.UserPasswordRecoveryConfirmation.Interactor;

internal sealed class UserWithRecoveredPassword(Guid id, string email)
{
    private readonly Guid _id = id;
    private readonly string _email = email;

    public async Task ResetPassword(
        StringHash hash,
        NpgsqlConnection connection,
        MailingBusPublisher publisher,
        CancellationToken ct = default
    )
    {
        string password = GenerateNewPassword();
        string hashed = HashPassword(hash, password);
        await UpdatePassword(hashed, connection, ct);
        await SendPasswordResetedMessage(password, publisher, ct);
    }

    private async Task UpdatePassword(
        string password,
        NpgsqlConnection connection,
        CancellationToken ct
    )
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = """
            UPDATE users_module.users SET password = @password WHERE id = @id 
            """;
        command.Parameters.Add(new NpgsqlParameter<string>("@password", password));
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", _id));
        int affected = await command.ExecuteNonQueryAsync(ct);
        if (affected == 0)
            throw new UserNotFoundException();
    }

    private static string HashPassword(StringHash hash, string password)
    {
        return hash.Hash(password.ToString());
    }

    private static string GenerateNewPassword()
    {
        Guid password = Guid.NewGuid();
        return password.ToString();
    }

    private async Task SendPasswordResetedMessage(
        string password,
        MailingBusPublisher publisher,
        CancellationToken ct
    )
    {
        string body = $"Новый пароль: {password}";
        MailingBusMessage message = new(_email, body, "Сброс пароля RemTech Агрегатор");
        await publisher.Send(message, ct);
    }
}
