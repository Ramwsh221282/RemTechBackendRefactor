using System.Data;
using Dapper;

namespace RemTech.SharedKernel.Infrastructure.NpgSql;

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
    
    public NpgSqlParametersStorage With<T, U>(string name, T source, Func<T,U> @object, DbType? dbType = null)
    {
        Parameters.Add(name, @object(source), dbType);
        return this;
    }

    public DynamicParameters GetParameters() => Parameters;
}

public sealed record ScopedParametersStorage<T>
{
    internal DynamicParameters Parameters { get; init; }
    
    private readonly T _value;

    internal ScopedParametersStorage(T value)
    {
        Parameters = new DynamicParameters();
        _value = value;
    }

    public ScopedParametersStorage<T> With(string name, T @object, DbType? dbType = null)
    {
        Parameters.Add(name, @object, dbType);
        return this;
    }
    
    public ScopedParametersStorage<T> WithNull(string name, DbType dbType)
    {
        Parameters.Add(name, DBNull.Value, dbType);
        return this;
    }
    
    public ScopedParametersStorage<T> WithValueOrNull(string name, Func<T,bool> predicate, Func<T,T> @object, DbType dbType)
    {
        if (!predicate(_value)) return WithNull(name, dbType);
        Parameters.Add(name, @object(_value), dbType);
        return this;
    }
    
    public ScopedParametersStorage<T> With(string name, Func<T,bool> predicate, Func<T,T> @object, DbType? dbType = null)
    {
        if (!predicate(_value)) return this;
        Parameters.Add(name, @object(_value), dbType);
        return this;
    }

    public ScopedParametersStorage<T> WithNullable<U>(string name, Func<T, U?> @object, DbType dbType)
    {
        U? item = @object(_value);
        if (item != null)
            Parameters.Add(name, item, dbType);
        if (item == null)
            Parameters.Add(name, DBNull.Value, dbType);
        return this;
    }
    
    public ScopedParametersStorage<T> With<U>(string name, Func<T,U> @object, DbType? dbType = null)
    {
        Parameters.Add(name, @object(_value), dbType);
        return this;
    }

    public ScopedParametersStorage<T> WithIfNotNull<U>(string name, Func<T, U?> @object, DbType dbType)
    {
        U? @item = @object(_value);
        return @item == null ? this : With(name, it => it, dbType);
    }

    public ScopedParametersStorage<T> WithIfNotNull<U,V>(
        string name, 
        Func<T, U?> @object, 
        Func<U, V> property, 
        DbType dbType)
    {
        U? @item = @object(_value);
        if (@item is null) return this;
        V field = @property(@item);
        Parameters.Add(name, field, dbType);
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

    extension<T>(ScopedParametersStorage<T>)
    {
        public static ScopedParametersStorage<T> New(T value)
        {
            return new ScopedParametersStorage<T>(value);
        }
    }
    
    extension(NpgSqlParametersStorage parametersStorage)
    {
        public CommandDefinition ToQuery(string sql, NpgSqlSession session, CancellationToken ct)
        {
            return new CommandDefinition(sql, parametersStorage.GetParameters(), cancellationToken: ct, transaction: session.Transaction);
        }
    }

    public static ScopedParametersStorage<T> Scoped<T>(T value)
    {
        return new ScopedParametersStorage<T>(value);
    }
}