using System.Text;
using Npgsql;

namespace Users.Module.Features.AuthenticateUser.Storage;

internal sealed class ByEmailUserReceivingMethod(string? email)
    : IUsersReceivingMethod<NpgsqlCommand, NpgsqlCommand>
{
    public NpgsqlCommand ModifyQuery(NpgsqlCommand query)
    {
        if (string.IsNullOrWhiteSpace(email))
            return query;
        StringBuilder stringBuilder = new(query.CommandText);
        stringBuilder.AppendLine(" WHERE email = @email ");
        query.CommandText = stringBuilder.ToString();
        query.Parameters.Add(new NpgsqlParameter<string>("@email", email));
        return query;
    }
}
