using System.Text;
using Npgsql;

namespace Users.Module.Features.AuthenticateUser.Storage;

internal sealed class ByNameUserReceivingMethod(string? name)
    : IUsersReceivingMethod<NpgsqlCommand, NpgsqlCommand>
{
    public NpgsqlCommand ModifyQuery(NpgsqlCommand query)
    {
        if (string.IsNullOrWhiteSpace(name))
            return query;
        StringBuilder stringBuilder = new(query.CommandText);
        stringBuilder.AppendLine(" WHERE name = @name ");
        query.CommandText = stringBuilder.ToString();
        query.Parameters.Add(new NpgsqlParameter<string>("@name", name));
        return query;
    }
}
