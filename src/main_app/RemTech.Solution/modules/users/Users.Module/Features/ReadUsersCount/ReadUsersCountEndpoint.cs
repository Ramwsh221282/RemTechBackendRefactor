using System.Data.Common;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using StackExchange.Redis;
using Users.Module.Models;

namespace Users.Module.Features.ReadUsersCount;

public static class ReadUsersCountEndpoint
{
    public static readonly Delegate HandleFn = Handle;

    private const string Sql = """
        SELECT COUNT(u.id) as total_count
        FROM users_module.users u
        LEFT JOIN users_module.user_roles ur ON u.id = ur.user_id
        LEFT JOIN users_module.roles r ON ur.role_id = r.id
        /*where*/
        """;

    private static async Task<IResult> Handle(
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromHeader(Name = "RemTechAccessTokenId")] string token,
        [FromQuery] string? nameFilter,
        [FromQuery] string? emailFilter,
        [FromQuery] string? roleFilter,
        CancellationToken ct
    )
    {
        try
        {
            if (!Guid.TryParse(token, out Guid tokenId))
                return Results.BadRequest(
                    new { message = "Ошибка авторизации. Попробуйте авторизоваться снова." }
                );
            Guid idOfCurrentReader = await GetIdOfCurrentReader(multiplexer, tokenId);
            List<string> filters = [];
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            await using NpgsqlCommand command = connection.CreateCommand();
            StringBuilder queryBuilder = BeginInitialUsersQuery();
            command.ApplyRoleFilterIsSpecified(filters, roleFilter);
            command.ApplyEmailFilterIfSpecified(filters, emailFilter);
            command.ApplyNameFilterIfSpecified(filters, nameFilter);
            command.IgnoreCurrentUser(filters, idOfCurrentReader);
            queryBuilder = queryBuilder.ApplyFilters(filters);
            command.CommandText = queryBuilder.ToString();
            await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
            await reader.ReadAsync(ct);
            long amount = reader.GetInt64(0);
            return Results.Ok(amount);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance} {Ex}", nameof(ReadUsersCountEndpoint), ex.Message);
            return Results.InternalServerError(new { message = ex.Message });
        }
    }

    private static StringBuilder BeginInitialUsersQuery()
    {
        return new StringBuilder(Sql);
    }

    private static void IgnoreCurrentUser(
        this NpgsqlCommand command,
        List<string> filters,
        Guid currentUser
    )
    {
        filters.Add("u.id != @currentUserId");
        command.Parameters.Add(new NpgsqlParameter<Guid>("@currentUserId", currentUser));
    }

    private static StringBuilder ApplyFilters(this StringBuilder builder, List<string> filters)
    {
        builder = builder.Replace("/*where*/", $"WHERE {string.Join(" AND ", filters)}");
        return builder;
    }

    private static void ApplyNameFilterIfSpecified(
        this NpgsqlCommand command,
        List<string> filters,
        string? nameFilter
    )
    {
        if (string.IsNullOrEmpty(nameFilter))
            return;
        filters.Add("u.name ILIKE @nameFilter");
        command.Parameters.AddWithValue("@nameFilter", $"%{nameFilter}%");
    }

    private static void ApplyEmailFilterIfSpecified(
        this NpgsqlCommand command,
        List<string> filters,
        string? emailFilter
    )
    {
        if (string.IsNullOrEmpty(emailFilter))
            return;
        filters.Add("u.email ILIKE @emailFilter");
        command.Parameters.AddWithValue("@emailFilter", $"%{emailFilter}%");
    }

    private static void ApplyRoleFilterIsSpecified(
        this NpgsqlCommand command,
        List<string> filters,
        string? roleFilter
    )
    {
        if (string.IsNullOrWhiteSpace(roleFilter))
            return;
        filters.Add("r.name = @roleFilter");
        command.Parameters.Add(new NpgsqlParameter<string>("@roleFilter", roleFilter));
    }

    private static async Task<Guid> GetIdOfCurrentReader(
        ConnectionMultiplexer multiplexer,
        Guid tokenId
    )
    {
        UserJwt jwt = new UserJwt(tokenId);
        jwt = await jwt.Provide(multiplexer);
        UserJwtOutput output = jwt.MakeOutput();
        return output.UserId;
    }
}
