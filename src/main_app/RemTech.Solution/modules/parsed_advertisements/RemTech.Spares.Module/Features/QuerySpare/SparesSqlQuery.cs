using System.Text;
using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Spares.Module.Features.QuerySpare;

internal sealed class SparesSqlQuery
{
    private const int MaxPageSize = 50;
    private readonly NpgsqlCommand _command;
    private readonly string _query = string.Intern(
        """
        SELECT
        object
        FROM spares_module.spares
        WHERE 1=1
        """
    );

    private readonly Serilog.ILogger _logger;
    private string _pagination;
    private string _textSearch;

    public SparesSqlQuery(NpgsqlCommand command, Serilog.ILogger logger)
    {
        _pagination = string.Empty;
        _textSearch = string.Empty;
        _logger = logger;
        _command = command;
    }

    public NpgsqlCommand Command()
    {
        StringBuilder sb = new StringBuilder(_query);
        sb = sb.AppendLine(_textSearch);
        sb = sb.AppendLine(_pagination);
        _command.CommandText = sb.ToString();
        _logger.Information("Spares query: {Query}.", _command.CommandText);
        return _command;
    }

    public void ApplyPagination(int page)
    {
        int offset = (page - 1) * MaxPageSize;
        _pagination = " LIMIT @limit OFFSET @offset ";
        _command.Parameters.Add(new NpgsqlParameter<int>("@limit", MaxPageSize));
        _command.Parameters.Add(new NpgsqlParameter<int>("@offset", offset));
    }

    public void ApplyTextSearch(IEmbeddingGenerator generator, string text)
    {
        _textSearch = " ORDER BY embedding <=> @embedding ";
        _command.Parameters.AddWithValue("@embedding", new Vector(generator.Generate(text)));
    }
}
