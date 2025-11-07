using System.Data;
using Dapper;
using Mailing.Tests.CleanWriteTests.Models;

namespace Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;

public sealed class NpgSqlPostmanMetadata(Guid id, string email, string password)
{
    public void WriteTo(DynamicParameters parameters)
    {
        parameters.Add("@id", id, DbType.Guid);
        parameters.Add("@email", email, DbType.String);
        parameters.Add("@password", password, DbType.String);
    }

    internal static NpgSqlPostmanMetadata FromPostman(ITestPostman postman) =>
        postman.Transform(s => new NpgSqlPostmanMetadata(s.Id, s.Email, s.Password));
}