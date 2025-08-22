using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using Users.Module.Features.ChangingEmail.Exceptions;

namespace Users.Module.Features.ConfirmUserEmail;

internal sealed class ConfirmUserEmailHandler(
    NpgsqlDataSource dataSource,
    Serilog.ILogger logger,
    ConfirmationEmailsCache cache
) : ICommandHandler<ConfirmUserEmailCommand>
{
    private const string Sql = """
        UPDATE users_module.users SET email_confirmed = TRUE
        WHERE id = @id;
        """;

    public async Task Handle(ConfirmUserEmailCommand command, CancellationToken ct = default)
    {
        Guid userId = await cache.Confirm(command.ConfirmationId);
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<Guid>("@id", userId));
        int affected = await sqlCommand.ExecuteNonQueryAsync(ct);
        if (affected == 0)
            throw new UserFromConfirmationNotFoundException();
        await cache.Delete(command.ConfirmationId);
        logger.Information("User: {Id} confirmed email.", userId);
    }
}
