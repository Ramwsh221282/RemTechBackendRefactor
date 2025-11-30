using System.Data;
using Dapper;

namespace RemTech.SharedKernel.Infrastructure.NpgSql;

public static class DynamicParametersExtensions
{
    extension(DynamicParameters parameters)
    {
        public void AddParameter<T>(T? value, string name)
            where T : struct
        {
            DbType type = typeof(T).DispatchDbType();
            parameters.Add(name, value.HasValue ? value.Value : null, type);
        }
        
        public void AddParameter<T>(T? value, string name)
            where T : notnull
        {
            DbType type = typeof(T).DispatchDbType();
            parameters.Add(name, value, type);
        }
    }

    extension(Type type)
    {
        private DbType DispatchDbType()
        {
            return type switch
            {
                Type t when t == typeof(Guid) => DbType.Guid,
                Type t when t == typeof(string) => DbType.String,
                Type t when t == typeof(int) => DbType.Int32,
                Type t when t == typeof(long) => DbType.Int64,
                Type t when t == typeof(DateTime) => DbType.Date,
                _ => throw new NotSupportedException($"Parameter: {type.Name} is not supported by {nameof(DbType)}")
            };
        }
    }
}