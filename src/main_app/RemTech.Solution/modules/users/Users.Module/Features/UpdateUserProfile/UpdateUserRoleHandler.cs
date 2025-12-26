using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Cqrs;
using Users.Module.CommonAbstractions;

namespace Users.Module.Features.UpdateUserProfile;

internal sealed class UpdateUserRoleHandler(
    NpgsqlDataSource dataSource,
    StringHash hash,
    MailingBusPublisher publisher,
    HasSenderApi hasSenderApi
) : ICommandHandler<UpdateUserProfileCommand, UpdateUserProfileResult>
{
    public async Task<UpdateUserProfileResult> Handle(
        UpdateUserProfileCommand command,
        CancellationToken ct = default
    )
    {
        if (!await hasSenderApi.HasSender())
            throw new SendersAreNotAvailableYetException();
        UpdateUserContext context = new UpdateUserContext();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(ct);
        try
        {
            await UpdateUserEmailIfSpecified(command, context, connection, ct);
            await UpdateUserNameIfSpecified(command, context, connection, ct);
            await UpdateUserRoleIfSpecified(command, context, connection, ct);
            await GenerateUserPasswordIfRequired(command, context, connection, ct);
            await transaction.CommitAsync(ct);
        }
        catch (NpgsqlException ex)
        {
            await transaction.RollbackAsync(ct);
            string message = ex.Message;
            if (message.Contains("email"))
                throw new EmailDuplicateException();
            if (message.Contains("name"))
                throw new NameDuplicateException();
            throw;
        }

        UpdateUserProfileResult result = context.PrintResult(command);
        await context.SendEmailMessage(publisher, ct);
        return result;
    }

    private async Task UpdateUserEmailIfSpecified(
        UpdateUserProfileCommand command,
        UpdateUserContext context,
        NpgsqlConnection connection,
        CancellationToken ct
    )
    {
        UpdateUserDetails details = command.UpdateUserDetails;
        if (string.IsNullOrWhiteSpace(details.NewUserEmail))
        {
            context.AddEmail(command.PreviousDetails.UserEmail);
            return;
        }

        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        string sql = string.Intern("UPDATE users_module.users SET email = @email WHERE id = @id;");
        sqlCommand.CommandText = sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>("@email", details.NewUserEmail));
        sqlCommand.Parameters.Add(new NpgsqlParameter<Guid>("@id", command.PreviousDetails.UserId));
        await sqlCommand.ExecuteNonQueryAsync(ct);
        context.AddEmail(details.NewUserEmail);
    }

    private async Task UpdateUserNameIfSpecified(
        UpdateUserProfileCommand command,
        UpdateUserContext context,
        NpgsqlConnection connection,
        CancellationToken ct
    )
    {
        UpdateUserDetails details = command.UpdateUserDetails;
        if (string.IsNullOrWhiteSpace(details.NewUserName))
        {
            context.AddName(command.PreviousDetails.UserName);
            return;
        }

        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        string sql = string.Intern("UPDATE users_module.users SET name = @name WHERE id = @id;");
        sqlCommand.CommandText = sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>("@name", details.NewUserName));
        sqlCommand.Parameters.Add(new NpgsqlParameter<Guid>("@id", command.PreviousDetails.UserId));
        await sqlCommand.ExecuteNonQueryAsync(ct);
        context.AddName(details.NewUserName);
    }

    private async Task UpdateUserRoleIfSpecified(
        UpdateUserProfileCommand command,
        UpdateUserContext context,
        NpgsqlConnection connection,
        CancellationToken ct
    )
    {
        UpdateUserDetails details = command.UpdateUserDetails;
        if (string.IsNullOrWhiteSpace(details.NewUserRole))
        {
            context.AddRole(command.PreviousDetails.UserRole);
            return;
        }

        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = string.Intern(
            "SELECT r.id FROM users_module.roles r WHERE r.name = @name;"
        );
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>("@name", details.NewUserRole));
        DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
        {
            await reader.DisposeAsync();
            throw new RoleNotFoundException(details.NewUserRole);
        }

        Guid roleId = reader.GetGuid(0);
        await reader.DisposeAsync();
        sqlCommand.CommandText = string.Intern(
            "UPDATE users_module.user_roles SET role_id = @role_id WHERE user_id = @user_id;"
        );
        sqlCommand.Parameters.Clear();
        sqlCommand.Parameters.Add(new NpgsqlParameter<Guid>("@role_id", roleId));
        sqlCommand.Parameters.Add(
            new NpgsqlParameter<Guid>("@user_id", command.PreviousDetails.UserId)
        );
        await sqlCommand.ExecuteNonQueryAsync(ct);
        context.AddRole(details.NewUserRole);
    }

    private async Task GenerateUserPasswordIfRequired(
        UpdateUserProfileCommand command,
        UpdateUserContext context,
        NpgsqlConnection connection,
        CancellationToken ct
    )
    {
        UpdateUserDetails details = command.UpdateUserDetails;
        if (!details.IsPasswordUpdateRequired)
            return;
        string newPassword = Guid.NewGuid().ToString();
        string hashedPassword = hash.Hash(newPassword);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = string.Intern(
            "UPDATE users_module.users SET password = @password WHERE id = @id;"
        );
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>("@password", hashedPassword));
        sqlCommand.Parameters.Add(new NpgsqlParameter<Guid>("@id", command.PreviousDetails.UserId));
        await sqlCommand.ExecuteNonQueryAsync(ct);
        context.AddPassword(newPassword);
    }
}