using System.Data.Common;
using System.Text;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Users.Module.Models.Features.RolesSeeding;

internal sealed class RolesSeedingOnStartup(NpgsqlDataSource dataSource, Serilog.ILogger logger)
    : BackgroundService
{
    private const string Context = nameof(RolesSeedingOnStartup);

    private const string SeedSql = "INSERT INTO users_module.roles (id,name)";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.Information("{Context} attempt to seed users module roles.", Context);
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(
            stoppingToken
        );
        if (!await ShouldSeedRoles(connection))
        {
            logger.Information("{Context} users module roles seeding is not required.", Context);
            return;
        }
        await CreateRootRole(connection);
        string role1 = "USER";
        string role2 = "ADMIN";
        Guid role1Id = Guid.NewGuid();
        Guid role2Id = Guid.NewGuid();

        StringBuilder sb = new StringBuilder(SeedSql);
        sb = sb.AppendLine("VALUES (@id_1, @name_1), ");
        sb = sb.AppendLine(" (@id_2, @name_2) ");

        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sb.ToString();
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id_1", role1Id));
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id_2", role2Id));
        command.Parameters.Add(new NpgsqlParameter<string>("@name_1", role1));
        command.Parameters.Add(new NpgsqlParameter<string>("@name_2", role2));
        await command.ExecuteNonQueryAsync(stoppingToken);
        logger.Information("{Context} roles seeded.", Context);
    }

    private const string CountSql = "SELECT COUNT(*) as amount FROM users_module.roles;";

    private async Task<bool> ShouldSeedRoles(NpgsqlConnection connection)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = CountSql;
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        long amount = reader.GetInt64(0);
        return amount == 0;
    }

    private async Task CreateRootRole(NpgsqlConnection connection)
    {
        string sql = "INSERT INTO users_module.roles (id,name) VALUES(@id, @name)";
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", Guid.NewGuid()));
        command.Parameters.Add(new NpgsqlParameter<string>("@name", "ROOT"));
        await command.ExecuteNonQueryAsync(CancellationToken.None);
    }
}
