using System.Data.Common;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.OnStartup;

public sealed class CreateVectorsOnStartup(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator,
    Serilog.ILogger logger
)
{
    public async Task Create()
    {
        await GenerateVectorsForBrands();
        await GenerateVectorsForKinds();
        await GenerateVectorsForLocations();
        await GenerateVectorsForModels();
    }

    private async Task GenerateVectorsForModels()
    {
        string sql = string.Intern(
            "SELECT b.text as text FROM parsed_advertisements_module.vehicle_models b WHERE b.embedding IS NULL;"
        );

        string countSql = string.Intern(
            "SELECT COUNT(b.text) FROM parsed_advertisements_module.vehicle_models b WHERE b.embedding IS NULL;"
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand countCommand = connection.CreateCommand();
        countCommand.CommandText = countSql;
        long total = (long)(await countCommand.ExecuteScalarAsync())!;
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        Dictionary<string, float[]> items = [];
        if (!await reader.ReadAsync())
        {
            logger.Information("All models have embeddings.");
            return;
        }

        long count = 0;
        do
        {
            string text = reader.GetString(reader.GetOrdinal("text"));
            if (items.ContainsKey(text))
                return;
            float[] embeddings = generator.Generate(text);
            items.Add(text, embeddings);
            logger.Information(
                "Added item to create embedding model. {Count}/{Total}",
                count,
                total
            );
            count++;
        } while (await reader.ReadAsync());

        if (items.Count == 0)
            return;

        string updateSql = string.Intern(
            "UPDATE parsed_advertisements_module.vehicle_models SET embedding = @embedding WHERE text = @text"
        );
        logger.Information("Generating embeddings for models...");
        foreach (KeyValuePair<string, float[]> item in items)
        {
            await using NpgsqlCommand updateCommand = dataSource.CreateCommand();
            updateCommand.CommandText = updateSql;
            updateCommand.Parameters.Add(new NpgsqlParameter<string>("@text", item.Key));
            updateCommand.Parameters.AddWithValue("@embedding", new Vector(item.Value));
            await updateCommand.ExecuteNonQueryAsync();
        }
        logger.Information("Embeddings for models has been generated.");
    }

    private async Task GenerateVectorsForLocations()
    {
        string sql = string.Intern(
            "SELECT b.text as text FROM parsed_advertisements_module.geos b WHERE b.embedding IS NULL;"
        );
        string countSql = string.Intern(
            "SELECT COUNT(b.text) FROM parsed_advertisements_module.vehicle_models b WHERE b.embedding IS NULL;"
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand countCommand = connection.CreateCommand();
        countCommand.CommandText = countSql;
        long total = (long)(await countCommand.ExecuteScalarAsync())!;
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        Dictionary<string, float[]> items = [];
        if (!await reader.ReadAsync())
        {
            logger.Information("All locations have embeddings.");
            return;
        }

        long count = 0;
        do
        {
            string text = reader.GetString(reader.GetOrdinal("text"));
            if (items.ContainsKey(text))
                return;
            float[] embeddings = generator.Generate(text);
            items.Add(text, embeddings);
            logger.Information(
                "Added item to create embedding model. {Count}/{Total}",
                count,
                total
            );
            count++;
        } while (await reader.ReadAsync());

        if (items.Count == 0)
            return;

        string updateSql = string.Intern(
            "UPDATE parsed_advertisements_module.geos SET embedding = @embedding WHERE text = @text"
        );
        logger.Information("Generating embeddings for locations...");
        foreach (KeyValuePair<string, float[]> item in items)
        {
            await using NpgsqlCommand updateCommand = dataSource.CreateCommand();
            updateCommand.CommandText = updateSql;
            updateCommand.Parameters.Add(new NpgsqlParameter<string>("@text", item.Key));
            updateCommand.Parameters.AddWithValue("@embedding", new Vector(item.Value));
            await updateCommand.ExecuteNonQueryAsync();
        }
        logger.Information("Embeddings for locations has been generated.");
    }

    private async Task GenerateVectorsForKinds()
    {
        string sql = string.Intern(
            "SELECT b.text as text FROM parsed_advertisements_module.vehicle_kinds b WHERE b.embedding IS NULL;"
        );
        string countSql = string.Intern(
            "SELECT COUNT(b.text) FROM parsed_advertisements_module.vehicle_models b WHERE b.embedding IS NULL;"
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        await using NpgsqlCommand countCommand = connection.CreateCommand();
        countCommand.CommandText = countSql;
        long total = (long)(await countCommand.ExecuteScalarAsync())!;
        command.CommandText = sql;
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        Dictionary<string, float[]> items = [];
        if (!await reader.ReadAsync())
        {
            logger.Information("All kinds have embeddings.");
            return;
        }

        long count = 0;
        do
        {
            string text = reader.GetString(reader.GetOrdinal("text"));
            if (items.ContainsKey(text))
                return;
            float[] embeddings = generator.Generate(text);
            items.Add(text, embeddings);
            logger.Information(
                "Added item to create embedding model. {Count}/{Total}",
                count,
                total
            );
            count++;
        } while (await reader.ReadAsync());

        if (items.Count == 0)
            return;

        logger.Information("Generating embeddings for kinds...");
        string updateSql = string.Intern(
            "UPDATE parsed_advertisements_module.vehicle_kinds SET embedding = @embedding WHERE text = @text"
        );
        foreach (KeyValuePair<string, float[]> item in items)
        {
            await using NpgsqlCommand updateCommand = dataSource.CreateCommand();
            updateCommand.CommandText = updateSql;
            updateCommand.Parameters.Add(new NpgsqlParameter<string>("@text", item.Key));
            updateCommand.Parameters.AddWithValue("@embedding", new Vector(item.Value));
            await updateCommand.ExecuteNonQueryAsync();
        }
        logger.Information("Embeddings for kinds has been generated.");
    }

    private async Task GenerateVectorsForBrands()
    {
        string sql = string.Intern(
            "SELECT b.text as text FROM parsed_advertisements_module.vehicle_brands b WHERE b.embedding IS NULL;"
        );
        string countSql = string.Intern(
            "SELECT COUNT(b.text) FROM parsed_advertisements_module.vehicle_models b WHERE b.embedding IS NULL;"
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand countCommand = connection.CreateCommand();
        countCommand.CommandText = countSql;
        long total = (long)(await countCommand.ExecuteScalarAsync())!;
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        Dictionary<string, float[]> items = [];
        if (!await reader.ReadAsync())
        {
            logger.Information("All brands have embeddings.");
            return;
        }

        long count = 0;
        do
        {
            string text = reader.GetString(reader.GetOrdinal("text"));
            if (items.ContainsKey(text))
                return;
            float[] embeddings = generator.Generate(text);
            items.Add(text, embeddings);
            logger.Information(
                "Added item to create embedding model. {Count}/{Total}",
                count,
                total
            );
            count++;
        } while (await reader.ReadAsync());

        if (items.Count == 0)
            return;

        logger.Information("Generating embeddings for brands...");
        string updateSql = string.Intern(
            "UPDATE parsed_advertisements_module.vehicle_brands SET embedding = @embedding WHERE text = @text"
        );
        foreach (KeyValuePair<string, float[]> item in items)
        {
            await using NpgsqlCommand updateCommand = dataSource.CreateCommand();
            updateCommand.CommandText = updateSql;
            updateCommand.Parameters.Add(new NpgsqlParameter<string>("@text", item.Key));
            updateCommand.Parameters.AddWithValue("@embedding", new Vector(item.Value));
            await updateCommand.ExecuteNonQueryAsync();
        }
        logger.Information("Embeddings for brands has been generated.");
    }
}
