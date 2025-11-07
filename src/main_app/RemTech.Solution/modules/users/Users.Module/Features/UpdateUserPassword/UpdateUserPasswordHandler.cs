using System.Data.Common;
using Mailing.Moduled.Bus;
using Npgsql;
using RemTech.Core.Shared.Cqrs;
using Users.Module.CommonAbstractions;
using Users.Module.Features.ChangingEmail.Shared;
using Users.Module.Features.CreateEmailConfirmation;

namespace Users.Module.Features.UpdateUserPassword;

internal sealed class UpdateUserPasswordHandler(
    NpgsqlDataSource dataSource,
    MailingBusPublisher publisher,
    StringHash hash
) : ICommandHandler<UpdateUserPasswordCommand>
{
    private const string FetchUserSql =
        "SELECT password, email FROM users_module.users WHERE id = @id;";

    private const string UpdatePasswordSql =
        "UPDATE users_module.users SET password = @password WHERE id = @id";

    public async Task Handle(UpdateUserPasswordCommand command, CancellationToken ct = default)
    {
        new PasswordValidation(command.NewPassword).Validate();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(ct);
        try
        {
            NpgsqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = FetchUserSql;
            sqlCommand.Parameters.Add(new NpgsqlParameter<Guid>("@id", command.UserId));
            DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
            if (!await reader.ReadAsync(ct))
            {
                await reader.DisposeAsync();
                throw new UserNotFoundException();
            }

            string realPassword = reader.GetString(0);
            string email = reader.GetString(1);
            await reader.DisposeAsync();
            if (!new PasswordsVerification(command.InputPassword, realPassword).IsVerified())
                throw new PasswordInvalidException();

            string hashedPassword = hash.Hash(command.NewPassword);
            sqlCommand.CommandText = UpdatePasswordSql;
            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.Add(new NpgsqlParameter<Guid>("@id", command.UserId));
            sqlCommand.Parameters.Add(new NpgsqlParameter<string>("@password", hashedPassword));
            await sqlCommand.ExecuteNonQueryAsync(ct);
            await transaction.CommitAsync(ct);
            await new PasswordChangedEmailMessage(publisher).Send(email, ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}