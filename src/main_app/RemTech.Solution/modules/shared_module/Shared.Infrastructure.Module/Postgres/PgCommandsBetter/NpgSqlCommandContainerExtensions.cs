using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Shared.Infrastructure.Module.Postgres.PgCommandsBetter;

public static class NpgSqlCommandContainerExtensions
{
    public static NpgSqlCommandContainer Command(this NpgsqlConnection connection)
    {
        return new NpgSqlCommandContainer(connection.CreateCommand());
    }

    public static NpgSqlCommandContainer WithQueryString(
        this NpgSqlCommandContainer container,
        string queryString
    )
    {
        NpgsqlCommand current = container.Command;
        current.CommandText = queryString;
        return new NpgSqlCommandContainer(current);
    }

    public static NpgSqlCommandContainer WithParameter<T>(
        this NpgSqlCommandContainer container,
        string paramName,
        T param
    )
    {
        NpgsqlCommand current = container.Command;
        current.Parameters.Add(new NpgsqlParameter(paramName, param));
        return new NpgSqlCommandContainer(current);
    }

    public static NpgSqlCommandContainer WithVector(
        this NpgSqlCommandContainer command,
        IEmbeddingGenerator generator,
        string text,
        string embeddingField
    )
    {
        NpgsqlCommand current = command.Command;
        current.Parameters.AddWithValue(embeddingField, new Vector(generator.Generate(text)));
        return new NpgSqlCommandContainer(current);
    }
}
