using System.Globalization;
using Brands.Module.Types;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Brands.Module.Features.AddBrandsOnStartup;

internal sealed class SeedingBrandsOnStartup(
    Serilog.ILogger logger,
    IEmbeddingGenerator generator,
    NpgsqlDataSource dataSource
) : BackgroundService
{
    private const string Entrance = nameof(SeedingBrandsOnStartup);

    private static readonly string CsvFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "OnStartup",
        "brands.csv"
    );

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(
            stoppingToken
        );
        if (await AreBrandAlreadySeeded(connection))
        {
            logger.Information("{Entrance}. Brands already seeded.", Entrance);
            return;
        }

        logger.Information("{Entrance} seeding brands.", Entrance);
        IBrand[] brands = ReadBrands();
        await using var writer = await connection.BeginBinaryImportAsync(
            "COPY brands_module.brands(id, name, rating, embedding) FROM STDIN (FORMAT BINARY)",
            stoppingToken
        );
        foreach (IBrand brand in brands)
        {
            await writer.StartRowAsync(stoppingToken);
            await writer.WriteAsync(brand.Id, stoppingToken);
            await writer.WriteAsync(brand.Name, stoppingToken);
            await writer.WriteAsync(brand.Rating, stoppingToken);
            await writer.WriteAsync(new Vector(generator.Generate(brand.Name)), stoppingToken);
        }

        await writer.CompleteAsync(stoppingToken);
        logger.Information("{Entrance}. Brands seeded.", Entrance);
    }

    private IBrand[] ReadBrands()
    {
        CsvConfiguration configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
        using StreamReader reader = new StreamReader(CsvFilePath);
        using CsvReader csv = new CsvReader(reader, configuration);
        Dictionary<string, IBrand> brands = [];
        while (csv.Read())
        {
            CsvBrand record = csv.GetRecord<CsvBrand>();
            if (!brands.ContainsKey(record.name))
                brands.Add(record.name, new Brand(Guid.NewGuid(), record.name, 0));
        }

        return brands.Values.ToArray();
    }

    private async Task<bool> AreBrandAlreadySeeded(NpgsqlConnection connection)
    {
        string sql = string.Intern("SELECT COUNT(id) FROM brands_module.brands");
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        object? count = await command.ExecuteScalarAsync();
        long number = (long)count!;
        return number > 0;
    }
}
