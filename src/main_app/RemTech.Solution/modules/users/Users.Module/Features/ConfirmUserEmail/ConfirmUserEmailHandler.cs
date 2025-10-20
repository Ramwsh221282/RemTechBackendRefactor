using Npgsql;
using RemTech.Core.Shared.Cqrs;
using Shared.Infrastructure.Module.Postgres;
using Users.Module.Features.ChangingEmail;

namespace Users.Module.Features.ConfirmUserEmail;

internal sealed class ConfirmUserEmailHandler(
    PostgresDatabase dataSource,
    Serilog.ILogger logger,
    ConfirmationEmailsCache cache
) : ICommandHandler<ConfirmUserEmailEndpoint.ConfirmUserEmailCommand>
{
    private const string Sql = """
        UPDATE users_module.users SET email_confirmed = TRUE
        WHERE id = @id;
        """;

    public async Task Handle(
        ConfirmUserEmailEndpoint.ConfirmUserEmailCommand command,
        CancellationToken ct = default
    )
    {
        Guid userId = await cache.Confirm(command.ConfirmationId);

        await using NpgsqlConnection connection = await dataSource.DataSource.OpenConnectionAsync(
            ct
        );

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
