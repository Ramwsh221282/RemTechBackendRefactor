using System.Data;
using Dapper;

namespace RemTech.NpgSql.Abstractions;

public sealed record NpgSqlParametersStorage
{
    internal DynamicParameters Parameters { get; init; }

    internal NpgSqlParametersStorage()
    {
        Parameters = new DynamicParameters();
    }

    public NpgSqlParametersStorage With<T>(string name, T @object, DbType? dbType = null)
    {
        Parameters.Add(name, @object, dbType);
        return this;
    }
    
    public NpgSqlParametersStorage WithNull(string name, DbType dbType)
    {
        Parameters.Add(name, DBNull.Value, dbType);
        return this;
    }
    
    public NpgSqlParametersStorage WithValueOrNull<T>(string name, T source, Func<T,bool> predicate, Func<T,T> @object, DbType dbType)
    {
        if (!predicate(source)) return WithNull(name, dbType);
        Parameters.Add(name, @object(source), dbType);
        return this;
    }
    
    public NpgSqlParametersStorage With<T>(string name, T source, Func<T,bool> predicate, Func<T,T> @object, DbType? dbType = null)
    {
        if (!predicate(source)) return this;
        Parameters.Add(name, @object(source), dbType);
        return this;
    }

    public DynamicParameters GetParameters() => Parameters;
}

public static class NpgSqlParametersModule
{
    extension(NpgSqlParametersStorage)
    {
        public static NpgSqlParametersStorage New()
        {
            return new NpgSqlParametersStorage();
        }
    }

    extension(NpgSqlParametersStorage parametersStorage)
    {
        public CommandDefinition ToQuery(string sql, NpgSqlSession session, CancellationToken ct)
        {
            return new CommandDefinition(sql, parametersStorage.GetParameters(), cancellationToken: ct, transaction: session.Transaction);
        }
    }
}