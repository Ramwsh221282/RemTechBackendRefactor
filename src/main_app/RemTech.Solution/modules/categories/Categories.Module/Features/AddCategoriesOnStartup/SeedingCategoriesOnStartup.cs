using System.Globalization;
using Categories.Module.Types;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Categories.Module.Features.AddCategoriesOnStartup;

internal sealed class SeedingCategoriesOnStartup(
    Serilog.ILogger logger,
    IEmbeddingGenerator generator,
    NpgsqlDataSource dataSource
) : BackgroundService
{
    private const string Entrance = nameof(SeedingCategoriesOnStartup);

    private static readonly string CsvFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "OnStartup",
        "categories.csv"
    );

    private const string Sql =
        "COPY categories_module.categories(id, name, rating, embedding) FROM STDIN (FORMAT BINARY)";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(
            stoppingToken
        );
        if (await AreBrandAlreadySeeded(connection))
        {
            logger.Information("{Entrance}. Categories already seeded.", Entrance);
            return;
        }

        logger.Information("{Entrance}. Seeding categories.", Entrance);
        ICategory[] categories = ReadCategories();
        await using var writer = await connection.BeginBinaryImportAsync(Sql, stoppingToken);
        foreach (ICategory category in categories)
        {
            await writer.StartRowAsync(stoppingToken);
            await writer.WriteAsync(category.Id, stoppingToken);
            await writer.WriteAsync(category.Name, stoppingToken);
            await writer.WriteAsync(category.Rating, stoppingToken);
            await writer.WriteAsync(new Vector(generator.Generate(category.Name)), stoppingToken);
        }

        await writer.CompleteAsync(stoppingToken);
        logger.Information("{Entrance}. Categories seeded.", Entrance);
    }

    private ICategory[] ReadCategories()
    {
        CsvConfiguration configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
        using StreamReader reader = new StreamReader(CsvFilePath);
        using CsvReader csv = new CsvReader(reader, configuration);
        Dictionary<string, ICategory> brands = [];
        while (csv.Read())
        {
            CsvCategory record = csv.GetRecord<CsvCategory>();
            if (!brands.ContainsKey(record.name))
                brands.Add(record.name, new Category(Guid.NewGuid(), record.name, 0));
        }

        return brands.Values.ToArray();
    }

    private async Task<bool> AreBrandAlreadySeeded(NpgsqlConnection connection)
    {
        string sql = string.Intern("SELECT COUNT(id) FROM categories_module.categories");
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        object? count = await command.ExecuteScalarAsync();
        long number = (long)count!;
        return number > 0;
    }
}
