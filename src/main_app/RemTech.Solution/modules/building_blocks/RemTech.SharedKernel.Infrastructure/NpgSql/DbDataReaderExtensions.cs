using System.Data;
using System.Runtime.CompilerServices;

namespace RemTech.SharedKernel.Infrastructure.NpgSql;

public static class DbDataReaderExtensions
{
    public static T GetValue<T>(this IDataReader reader, string columnName)
    {
        Type requiredType = typeof(T);
        return requiredType switch
        {
            Type t when t == typeof(int) => GetInt32<T>(reader, columnName),
            Type t when t == typeof(long) => GetInt64<T>(reader, columnName),
            Type t when t == typeof(Guid) => GetGuid<T>(reader, columnName),
            Type t when t == typeof(string) => GetString<T>(reader, columnName),
            Type t when t == typeof(DateTime) => GetDateTime<T>(reader, columnName),
            _ => throw new NotSupportedException($"Unsupported type {requiredType.Name}")
        };
    }

    public static T? GetNullable<T>(this IDataReader reader, string columnName) where T : struct
    {
        Type requiredType = typeof(T);
        return requiredType switch
        {
            Type t when t == typeof(int) => reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : GetInt32<T>(reader, columnName),
            Type t when t == typeof(long) => reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : GetInt64<T>(reader, columnName),
            Type t when t == typeof(Guid) => reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : GetGuid<T>(reader, columnName),
            Type t when t == typeof(string) => reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : GetString<T>(reader, columnName),
            Type t when t == typeof(DateTime) => reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : GetDateTime<T>(reader, columnName),
            _ => throw new NotSupportedException($"Unsupported type {requiredType.Name}")
        };
    }
    
    private static T GetInt32<T>(IDataReader reader, string columnName)
    {
        int value = reader.GetInt32(reader.GetOrdinal(columnName));
        return Unsafe.As<int, T>(ref value);
    }
    
    private static T GetGuid<T>(IDataReader reader, string columnName)
    {
        Guid value = reader.GetGuid(reader.GetOrdinal(columnName));
        return Unsafe.As<Guid, T>(ref value);
    }
    
    private static T GetInt64<T>(IDataReader reader, string columnName)
    {
        long value = reader.GetInt64(reader.GetOrdinal(columnName));
        return Unsafe.As<long, T>(ref value);   
    }

    private static T GetDateTime<T>(IDataReader reader, string columnName)
    {
        DateTime value = reader.GetDateTime(reader.GetOrdinal(columnName));
        return Unsafe.As<DateTime, T>(ref value);
    }
    
    private static T GetString<T>(IDataReader reader, string columnName)
    {
        string value = reader.GetString(reader.GetOrdinal(columnName));
        return Unsafe.As<string, T>(ref value);
    }
}