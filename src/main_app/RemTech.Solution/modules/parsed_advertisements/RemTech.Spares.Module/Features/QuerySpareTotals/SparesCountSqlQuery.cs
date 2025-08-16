using System.Text;
using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Spares.Module.Features.QuerySpareTotals;

internal sealed class SparesCountSqlQuery(NpgsqlCommand command)
{
    private const int MaxPageSize = 50;

    private readonly string _query = string.Intern(
        """
        SELECT
        COUNT(s.id)
        FROM spares_module.spares s
        INNER JOIN contained_items.items c ON c.id = s.id
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
}
