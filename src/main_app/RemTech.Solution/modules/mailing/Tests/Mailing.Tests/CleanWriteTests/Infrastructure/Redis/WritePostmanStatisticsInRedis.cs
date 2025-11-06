namespace Mailing.Tests.CleanWriteTests.Infrastructure.Redis;

public sealed class WritePostmanStatisticsInRedis(JsonPostman postman) : IWritePostmanStatisticsInfrastructureCommand
{
    public void Execute(in int sendLimit, in int currentSend)
    {
        postman.AddLimitSend(sendLimit);
        postman.AddCurrentSend(currentSend);
    }
}