using Dapper;
using Mailing.Tests.CleanWriteTests.Models;

namespace Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;

public sealed class NpgSqlPostmanBody(DynamicParameters parameters)
{
    public DynamicParameters Parameters => parameters;

    internal static NpgSqlPostmanBody FromPostman(ITestPostman postman)
    {
        NpgSqlPostmanMetadata metadata = NpgSqlPostmanMetadata.FromPostman(postman);
        NpgSqlPostmanStatistics statistics = NpgSqlPostmanStatistics.FromPostman(postman);
        DynamicParameters parameters = new();
        metadata.WriteTo(parameters);
        statistics.WriteTo(parameters);
        return new NpgSqlPostmanBody(parameters);
    }
}