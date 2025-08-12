using System.Text;
using Npgsql;

namespace Users.Module.Features.AuthenticateUser.Storage;

internal sealed class ByEmailUserReceivingMethod(string? email)
    : IUsersReceivingMethod<NpgsqlCommand, NpgsqlCommand>
{
    private const string QueryPart = " WHERE email = @email ";
    private const string EmailParam = "@email";

    public NpgsqlCommand ModifyQuery(NpgsqlCommand query)
    {
        if (string.IsNullOrWhiteSpace(email))
            return query;
        StringBuilder stringBuilder = new(query.CommandText);
        stringBuilder.AppendLine(QueryPart);
        query.CommandText = stringBuilder.ToString();
        query.Parameters.Add(new NpgsqlParameter<string>(EmailParam, email));
        return query;
    }
}
