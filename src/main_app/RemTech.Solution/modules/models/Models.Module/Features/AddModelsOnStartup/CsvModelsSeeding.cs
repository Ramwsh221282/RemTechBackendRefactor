using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Hosting;
using Models.Module.Types;
using Npgsql;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Models.Module.Features.AddModelsOnStartup;

internal sealed class CsvModelsSeeding(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator,
    Serilog.ILogger logger
) : BackgroundService
{
    private const string Entrance = nameof(CsvModelsSeeding);

    private static readonly string CsvFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "OnStartup",
        "models.csv"
    );

    private const string Sql =
        "COPY models_module.models(id, name, rating, embedding) FROM STDIN (FORMAT BINARY)";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(
        //     stoppingToken
        // );
        // if (await AreBrandAlreadySeeded(connection))
        // {
        //     logger.Information("{Entrance}. Models already seeded.", Entrance);
        //     return;
        // }
        //
        // logger.Information("{Entrance}. Seeding models.", Entrance);
        // IModel[] models = ReadModels();
        // await using var writer = await connection.BeginBinaryImportAsync(Sql, stoppingToken);
        // foreach (IModel model in models)
        // {
        //     await writer.StartRowAsync(stoppingToken);
        //     await writer.WriteAsync(model.Id, stoppingToken);
        //     await writer.WriteAsync(model.Name, stoppingToken);
        //     await writer.WriteAsync(model.Rating, stoppingToken);
        //     await writer.WriteAsync(new Vector(generator.Generate(model.Name)), stoppingToken);
        // }
        //
        // await writer.CompleteAsync(stoppingToken);
        // logger.Information("{Entrance}. Models seeded.", Entrance);
    }

    private IModel[] ReadModels()
    {
        CsvConfiguration configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
        using StreamReader reader = new StreamReader(CsvFilePath);
        using CsvReader csv = new CsvReader(reader, configuration);
        Dictionary<string, IModel> models = [];
        while (csv.Read())
        {
            CsvModel record = csv.GetRecord<CsvModel>();
            if (!models.ContainsKey(record.name))
                models.Add(record.name, new Model(Guid.NewGuid(), record.name, 0));
        }

        return models.Values.ToArray();
    }

    private async Task<bool> AreBrandAlreadySeeded(NpgsqlConnection connection)
    {
        string sql = string.Intern("SELECT COUNT(id) FROM models_module.models");
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        object? count = await command.ExecuteScalarAsync();
        long number = (long)count!;
        return number > 0;
    }
}
