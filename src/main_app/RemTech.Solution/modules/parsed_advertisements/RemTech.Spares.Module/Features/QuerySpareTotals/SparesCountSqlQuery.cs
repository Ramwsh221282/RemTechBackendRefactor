using System.Text;
using Npgsql;
using Pgvector;
using RemTech.Vehicles.Module.Database.Embeddings;

namespace RemTech.Spares.Module.Features.QuerySpareTotals;

internal sealed class SparesCountSqlQuery(NpgsqlCommand command)
{
    private const int MaxPageSize = 50;

    private readonly string _query = string.Intern(
        """
        SELECT
        COUNT(id)
        FROM spares_module.spares
        WHERE 1=1
        """
    );

    private string _textSearch = string.Empty;

    public NpgsqlCommand Command()
    {
        StringBuilder sb = new StringBuilder(_query);
        sb = sb.AppendLine(_textSearch);
        command.CommandText = sb.ToString();
        return command;
    }

    public void ApplyTextSearch(IEmbeddingGenerator generator, string text)
    {
        _textSearch = " ORDER BY embedding <=> @embedding ";
        command.Parameters.AddWithValue("@embedding", new Vector(generator.Generate(text)));
    }
}
