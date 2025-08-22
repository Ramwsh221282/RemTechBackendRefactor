using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Users.Module.Features.ReadRoles;

public static class ReadRolesEndpoint
{
    public static readonly Delegate HandleFn = Handle;

    internal sealed record ReadRolesResponse(string Name);

    private const string Sql = "SELECT r.name FROM users_module.roles r";

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
            await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
            List<ReadRolesResponse> response = [];
            while (await reader.ReadAsync(ct))
            {
                string name = reader.GetString(0);
                response.Add(new ReadRolesResponse(name));
            }

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(ReadRolesEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
