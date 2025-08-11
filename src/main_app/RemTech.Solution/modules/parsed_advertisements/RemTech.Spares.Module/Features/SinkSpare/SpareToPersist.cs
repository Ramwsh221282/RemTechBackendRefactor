using Npgsql;
using RemTech.Spares.Module.Features.SinkSpare.Exceptions;
using RemTech.Spares.Module.Features.SinkSpare.Json;
using RemTech.Spares.Module.Features.SinkSpare.Models;
using RemTech.Spares.Module.Features.SinkSpare.Persistance;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Spares.Module.Features.SinkSpare;

internal sealed class SpareToPersist(
    Spare spare,
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator
)
{
    public SpareToPersist(
        SpareSinkMessage message,
        SpareLocation location,
        NpgsqlDataSource dataSource,
        IEmbeddingGenerator generator
    )
        : this(
            new Spare(
                message.Spare.Id,
                new SpareTextInformation(message.Spare.Description, message.Spare.Title),
                location,
                new SparePrice(message.Spare.PriceValue, message.Spare.IsNds),
                new SpareSourceInformation(message.Spare.SourceUrl, message.Parser.ParserDomain),
                new SparePhotos(message.Spare.Photos)
            ),
            dataSource,
            generator
        ) { }

    public async Task Store(CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            INSERT INTO spares_module.spares
            (id, region_id, city_id, price, is_nds, source_url, source_domain, object, embedding)
            VALUES
            (@id, @region_id, @city_id, @price, @is_nds, @source_url, @source_domain, @object, @embedding)
            ON CONFLICT(id) DO NOTHING;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        SpareSqlPersistanceCommand persistanceCommand = new SpareSqlPersistanceCommand(command);
        SpareEmbeddingStringSource embeddingStringSource = new(generator);
        SpareJsonObject json = new SpareJsonObject();
        spare.Modify(embeddingStringSource);
        spare.Modify(json);
        persistanceCommand = spare.Modify(persistanceCommand);
        persistanceCommand = embeddingStringSource.Modify(persistanceCommand);
        persistanceCommand = json.Modify(persistanceCommand);
        int affected = await persistanceCommand.Read().ExecuteNonQueryAsync(ct);
        if (affected == 0)
            throw new SparePersistDuplicateIdException();
    }
}
