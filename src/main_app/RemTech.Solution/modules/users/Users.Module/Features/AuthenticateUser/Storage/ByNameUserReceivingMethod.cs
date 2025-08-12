using System.Text;
using Npgsql;

namespace Users.Module.Features.AuthenticateUser.Storage;

internal sealed class ByNameUserReceivingMethod(string? name)
    : IUsersReceivingMethod<NpgsqlCommand, NpgsqlCommand>
{
    private const string QueryPart = " WHERE name = @name ";
    private const string NameParam = "@name";

    public NpgsqlCommand ModifyQuery(NpgsqlCommand query)
    {
        if (string.IsNullOrWhiteSpace(name))
            return query;
        StringBuilder stringBuilder = new(query.CommandText);
        stringBuilder.AppendLine(QueryPart);
        query.CommandText = stringBuilder.ToString();
        query.Parameters.Add(new NpgsqlParameter<string>(NameParam, name));
        return query;
    }
}
