using System.Data.Common;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using StackExchange.Redis;
using Users.Module.Features.VerifyingAdmin;
using Users.Module.Models;

namespace Users.Module.Features.ReadUsers;

public static class ReadUsersEndpoint
{
    public static Delegate HandleFn = Handle;

    internal sealed record ReadUsersResponse(
        Guid Id,
        string Email,
        string Name,
        bool EmailConfirmed,
        string Role
    );

    private const int PageSize = 30;

    private const string FetchUsersSql = """
        SELECT
            u.id as user_id,
            u.name as user_name,
            u.email as user_email,
            u.email_confirmed as user_email_confirmed,
            r.name as role_name
        FROM users_module.users u
        LEFT JOIN users_module.user_roles ur ON u.id = ur.user_id
        LEFT JOIN users_module.roles r ON ur.role_id = r.id
        /*where*/
        /*limit*/
        /*offset*/
        """;

    private static async Task<IResult> Handle(
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromHeader(Name = "RemTechAccessTokenId")] string token,
        [FromQuery] string? nameFilter,
        [FromQuery] string? emailFilter,
        [FromQuery] string? roleFilter,
        [FromQuery] int page,
        CancellationToken ct
    )
    {
        try
        {
            if (page <= 0)
                return Results.Ok(Enumerable.Empty<ReadUsersResponse>());
            if (!Guid.TryParse(token, out Guid tokenId))
                return Results.BadRequest(
                    new { message = "Ошибка авторизации. Попробуйте авторизоваться снова." }
                );
            Guid idOfCurrentReader = await GetIdOfCurrentReader(multiplexer, tokenId);
            List<string> filters = [];
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            await using NpgsqlCommand command = connection.CreateCommand();
            StringBuilder queryBuilder = BeginInitialUsersQuery().ApplyPagination(command, page);
            command.ApplyRoleFilterIsSpecified(filters, roleFilter);
            command.ApplyEmailFilterIfSpecified(filters, emailFilter);
            command.ApplyNameFilterIfSpecified(filters, nameFilter);
            command.IgnoreCurrentUser(filters, idOfCurrentReader);
            queryBuilder = queryBuilder.ApplyFilters(filters);
            command.CommandText = queryBuilder.ToString();
            await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
            if (!reader.HasRows)
                return Results.Ok(Enumerable.Empty<ReadUsersResponse>());
            List<ReadUsersResponse> response = [];
            while (await reader.ReadAsync(ct))
            {
                Guid userId = reader.GetGuid(reader.GetOrdinal("user_id"));
                string userName = reader.GetString(reader.GetOrdinal("user_name"));
                string userEmail = reader.GetString(reader.GetOrdinal("user_email"));
                bool emailConfirmed = reader.GetBoolean(reader.GetOrdinal("user_email_confirmed"));
                string roleName = reader.GetString(reader.GetOrdinal("role_name"));
                response.Add(
                    new ReadUsersResponse(userId, userEmail, userName, emailConfirmed, roleName)
                );
            }
            return Results.Ok(response);
        }
        catch (UnableToGetUserJwtValueException)
        {
            return Results.BadRequest(
                new { message = "Ошибка авторизации. Попробуйте авторизоваться снова." }
            );
        }
        catch (UserJwtTokenComparisonDifferentException)
        {
            return Results.BadRequest(
                new { message = "Ошибка авторизации. Попробуйте авторизоваться снова." }
            );
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(ReadUsersEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }

    private static StringBuilder BeginInitialUsersQuery()
    {
        return new StringBuilder(FetchUsersSql);
    }

    private static StringBuilder ApplyFilters(this StringBuilder builder, List<string> filters)
    {
        builder = builder.Replace("/*where*/", $"WHERE {string.Join(" AND ", filters)}");
        return builder;
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

    private static StringBuilder ApplyPagination(
        this StringBuilder query,
        NpgsqlCommand command,
        int page
    )
    {
        int offset = (page - 1) * PageSize;
        query = query.Replace("/*limit*/", "LIMIT @limit").Replace("/*offset*/", "OFFSET @offset");
        command.Parameters.Add(new NpgsqlParameter<int>("@limit", PageSize));
        command.Parameters.Add(new NpgsqlParameter<int>("@offset", offset));
        return query;
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
