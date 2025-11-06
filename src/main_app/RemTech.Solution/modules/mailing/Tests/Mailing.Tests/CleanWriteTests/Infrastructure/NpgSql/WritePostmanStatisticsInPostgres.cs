using System.Data;
using Dapper;

namespace Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;

public sealed class WritePostmanStatisticsInPostgres(DynamicParameters parameters)
    : IWritePostmanStatisticsInfrastructureCommand
{
    public void Execute(in int sendLimit, in int currentSend) =>
        AddToCommandParameters(sendLimit, currentSend);

    private void AddToCommandParameters(in int sendLimit, in int currentSend)
    {
        parameters.Add("@send_limit", sendLimit, DbType.Int32);
        parameters.Add("@send_current", currentSend, DbType.Int32);
    }
}