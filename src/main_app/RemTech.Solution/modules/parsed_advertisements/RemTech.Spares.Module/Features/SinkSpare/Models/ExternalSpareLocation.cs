using System.Data.Common;
using Npgsql;
using Pgvector;
using RemTech.Spares.Module.Features.SinkSpare.Exceptions;
using RemTech.Vehicles.Module.Database.Embeddings;

namespace RemTech.Spares.Module.Features.SinkSpare.Models;

internal sealed class ExternalSpareLocation(
    string locationText,
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator
) : IExternalSpareLocation
{
    public async Task<SpareLocation> Fetch(CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            SELECT id, text, kind FROM parsed_advertisements_module.geos ORDER BY embedding <=> @embedding LIMIT 1;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@embedding", new Vector(generator.Generate(locationText)));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new SpareJsonObjectModifierValueEmptyException(nameof(locationText));
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string name = reader.GetString(reader.GetOrdinal("text"));
        string kind = reader.GetString(reader.GetOrdinal("kind"));
        return new SpareLocation(id, name, kind);
    }
}
