using Mailing.Module.Bus;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using Users.Module.Features.CreateEmailConfirmation;

namespace Users.Module.Features.RemoveUserByAdmin;

internal sealed class RemoveUserByAdminHandler(
    NpgsqlDataSource dataSource,
    MailingBusPublisher publisher
) : ICommandHandler<RemoveUserByAdminCommand>
{
    private const string Sql = "DELETE FROM users_module.users WHERE id = @id RETURNING email;";

    public async Task Handle(RemoveUserByAdminCommand command, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(ct);
        try
        {
            await using NpgsqlCommand npgsqlCommand = connection.CreateCommand();
            npgsqlCommand.CommandText = "DELETE FROM users_module.user_roles WHERE user_id = @id";
            npgsqlCommand.Parameters.Add(new NpgsqlParameter<Guid>("@id", command.UserId));
            await npgsqlCommand.ExecuteNonQueryAsync(ct);
            npgsqlCommand.CommandText = Sql;
            npgsqlCommand.Parameters.Add(new NpgsqlParameter<Guid>("@id", command.UserId));
            NpgsqlDataReader reader = await npgsqlCommand.ExecuteReaderAsync(ct);
            if (!await reader.ReadAsync(ct))
            {
                await reader.DisposeAsync();
                throw new UserNotFoundException();
            }
            string email = reader.GetString(0);
            await reader.DisposeAsync();
            MailingBusMessage message = new MailingBusMessage(
                email,
                "Учетная запись на RemTech аггрегатор спец. техники была удалена.",
                "Удаление учетной записи RemTech"
            );
            await transaction.CommitAsync(ct);
            try
            {
                await publisher.Send(message, ct);
            }
            catch
            {
                // ignored
            }
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
