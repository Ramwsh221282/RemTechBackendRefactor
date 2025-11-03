using System.Data;

namespace RemTech.Core.Shared.Database;

public static class IDbCommandExtensions
{
    public static IDbCommand AddParameter(this IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
        return command;
    }

    public static IDbCommand AddParameter(this IDbCommand command, string name, object value, DbType type)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        parameter.DbType = type;
        command.Parameters.Add(parameter);
        return command;
    }
}