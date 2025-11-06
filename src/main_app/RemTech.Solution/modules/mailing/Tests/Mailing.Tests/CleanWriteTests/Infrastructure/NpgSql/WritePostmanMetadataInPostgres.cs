using System.Data;
using Dapper;

namespace Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;

public sealed class WritePostmanMetadataInPostgres(DynamicParameters parameters)
    : IWritePostmanMetadataInfrastructureCommand
{
    public void Execute(in Guid id, in string email, in string password) =>
        AddToCommandParameters(id, email, password);

    private void AddToCommandParameters(in Guid id, in string email, in string password)
    {
        parameters.Add("@id", id, DbType.Guid);
        parameters.Add("@email", email, DbType.String);
        parameters.Add("@password", password, DbType.String);
    }
}