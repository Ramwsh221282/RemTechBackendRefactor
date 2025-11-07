using System.Data;
using Dapper;
using Mailing.Tests.CleanWriteTests.Models;

namespace Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;

public sealed class NpgSqlPostmanStatistics(int sendLimit, int currentSend)
{
    public void WriteTo(DynamicParameters parameters)
    {
        parameters.Add("@current_sent", sendLimit, DbType.Int32);
        parameters.Add("@current_limit", currentSend, DbType.Int32);
    }

    internal static NpgSqlPostmanStatistics FromPostman(ITestPostman postman) =>
        postman.Transform(s => new NpgSqlPostmanStatistics(s.LimitSend, s.CurrentSend));
}