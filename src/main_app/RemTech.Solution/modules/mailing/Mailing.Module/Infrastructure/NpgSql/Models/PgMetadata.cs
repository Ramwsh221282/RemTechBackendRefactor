using Dapper;
using System.Data;
using RemTech.Core.Shared.Reflection;

namespace Mailing.Module.Infrastructure.NpgSql.Models;

internal sealed class PgMetadata
{
    private readonly Guid _id;
    private readonly string _email;
    private readonly string _password;

    public DynamicParameters WriteTo(DynamicParameters? parameters)
    {
        parameters ??= new DynamicParameters();
        parameters.Add("@id", _id, DbType.Guid);
        parameters.Add("@email", _email, DbType.String);
        parameters.Add("@password", _password, DbType.String);
        return parameters;
    }

    public PgMetadata(IMailer postman)
    {
        FieldsSearcher searcher = new(postman);
        _id = searcher.SearchByName<Guid>(nameof(_id));
        _email = searcher.SearchByName<string>(nameof(_email));
        _password = searcher.SearchByName<string>(nameof(_password));
    }
}