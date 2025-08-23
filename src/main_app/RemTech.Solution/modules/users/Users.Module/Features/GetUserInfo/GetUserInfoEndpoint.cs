using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using StackExchange.Redis;
using Users.Module.Features.VerifyingAdmin;
using Users.Module.Models;

namespace Users.Module.Features.GetUserInfo;

public static class GetUserInfoEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("info", Handle);

    internal sealed record UserInfoResponse(
        Guid Id,
        string Name,
        string Email,
        bool EmailConfirmed
    );

    private const string Sql = """
        SELECT id, name, email, email_confirmed
        FROM users_module.users WHERE id = @id;
        """;

    private static async Task<IResult> Handle(
        [FromServices] Serilog.ILogger logger,
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromHeader(Name = "RemTechAccessTokenId")] string token,
        CancellationToken ct
    )
    {
        try
        {
            if (!Guid.TryParse(token, out Guid tokenId))
                return Results.BadRequest(
                    new { message = "Ошибка авторизации. Попробуйте авторизоваться снова." }
                );
            UserJwt jwt = new UserJwt(tokenId);
            jwt = await jwt.Provide(multiplexer);
            UserJwtOutput output = jwt.MakeOutput();
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            await using NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = Sql;
            command.Parameters.Add(new NpgsqlParameter<Guid>("@id", output.UserId));
            await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
            if (!await reader.ReadAsync(ct))
                return Results.BadRequest(new { message = "Пользователь не был найден." });
            Guid id = reader.GetGuid(0);
            string name = reader.GetString(1);
            string email = reader.GetString(2);
            bool emailConfirmed = reader.GetBoolean(3);
            UserInfoResponse response = new(id, name, email, emailConfirmed);
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
            logger.Fatal("{Entrance} {Ex}", nameof(GetUserInfoEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}
