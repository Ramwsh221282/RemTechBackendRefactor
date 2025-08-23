using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Users.Module.Features.CreatingNewAccount;

namespace Users.Module.Features.CheckRoot;

public static class EnsureRootCreatedEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("root-get", Handle);

    private const string Sql = """
        SELECT (EXISTS (
            SELECT 1 
            FROM users_module.users u
            INNER JOIN users_module.user_roles ur ON u.id = ur.user_id
            INNER JOIN users_module.roles r ON ur.role_id = r.id
            WHERE r.name = 'ROOT'
        ))::int AS has_root_user;
        """;

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        try
        {
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            await using NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = Sql;
            object? result = await command.ExecuteScalarAsync(ct);
            int number = (int)result!;
            return Results.Ok(number != 0);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(CreateUserAccountEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}
