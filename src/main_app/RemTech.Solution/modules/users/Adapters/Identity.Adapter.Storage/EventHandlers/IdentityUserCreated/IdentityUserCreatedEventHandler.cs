using System.Data;
using System.Data.Common;
using Dapper;
using Identity.Domain.Users.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Identity.Adapter.Storage.EventHandlers.IdentityUserCreated;

public sealed class IdentityUserCreatedEventHandler(
    Serilog.ILogger logger,
    IdentityDbContext context
) : IDomainEventHandler<IdentityUserCreatedEvent>
{
    private const string Context = nameof(IdentityUserCreatedEvent);

    public async Task<Status> Handle(
        IdentityUserCreatedEvent @event,
        CancellationToken ct = default
    )
    {
        DbConnection connection = context.Database.GetDbConnection();
        await using IDbContextTransaction txn = await context.Database.BeginTransactionAsync(ct);
        try
        {
            IEnumerable<Guid> roleIds = @event.Roles.Select(r => r.Id);
            await InsertUser(@event.UserId, @event.Profile, connection, txn, ct);
            await InsertUserRoles(@event.UserId, roleIds, connection, txn, ct);
            await txn.CommitAsync(ct);
            return Status.Success();
        }
        catch (Exception ex)
        {
            logger.Error("{Context}. Error: {ErrorMessage}.", Context, ex.Message);
            await txn.RollbackAsync(ct);
            throw;
        }
    }

    private async Task InsertUser(
        Guid userId,
        IdentityUserProfileEventArgs profile,
        DbConnection connection,
        IDbContextTransaction txn,
        CancellationToken ct = default
    )
    {
        const string sql = """
            INSERT INTO users_module.users
            (id, name, password, email, email_confirmed)
            VALUES
            (@id, @name, @password, @email, @email_confirmed)
            """;

        CommandDefinition command = new(
            sql,
            new
            {
                id = userId,
                name = profile.UserLogin,
                password = profile.UserPassword,
                email = profile.UserEmail,
                email_confirmed = profile.EmailConfirmed,
            },
            cancellationToken: ct,
            transaction: txn.GetDbTransaction()
        );

        await connection.ExecuteAsync(command);
    }

    private async Task InsertUserRoles(
        Guid userId,
        IEnumerable<Guid> roleIds,
        IDbConnection connection,
        IDbContextTransaction txn,
        CancellationToken ct = default
    )
    {
        const string sql = """
            INSERT INTO users_module.user_roles
            (user_id, role_id)
            VALUES
            (@user_id, @role_id)
            """;

        var parameters = roleIds.Select(r => new { user_id = userId, role_id = r });
        CommandDefinition command = new(
            sql,
            parameters,
            cancellationToken: ct,
            transaction: txn.GetDbTransaction()
        );
        await connection.ExecuteAsync(command);
    }
}
