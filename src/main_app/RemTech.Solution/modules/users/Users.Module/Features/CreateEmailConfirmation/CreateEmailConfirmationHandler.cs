using System.Data.Common;
using Mailing.Module.Bus;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Frontend;
using StackExchange.Redis;
using Users.Module.Features.ChangingEmail.Exceptions;
using Users.Module.Features.ChangingEmail.Shared;

namespace Users.Module.Features.CreateEmailConfirmation;

internal sealed class CreateEmailConfirmationHandler(
    ConnectionMultiplexer multiplexer,
    NpgsqlDataSource dataSource,
    FrontendUrl frontendUrl,
    MailingBusPublisher publisher
) : ICommandHandler<CreateEmailConfirmationCommand, Guid>
{
    private const string Sql = """
        SELECT email, email_confirmed, password
        FROM users_module.users WHERE id = @id;
        """;

    public async Task<Guid> Handle(
        CreateEmailConfirmationCommand command,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<Guid>("@id", command.UserId));
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new UserNotFoundException();
        string email = reader.GetString(0);
        bool emailConfirmed = reader.GetBoolean(1);
        string password = reader.GetString(2);
        if (!new PasswordsVerification(command.InputPassword, password).IsVerified())
            throw new PasswordInvalidException();
        if (emailConfirmed)
            throw new EmailIsAlreadyConfirmedException();
        Guid confirmationKey = await new EmailConfirmationKeyGeneration(
            new ConfirmationEmailsCache(multiplexer)
        ).Generate(command.UserId);
        await new EmailConfirmationMailingMessage(frontendUrl, confirmationKey, publisher).Send(
            email,
            ct
        );
        return confirmationKey;
    }
}
