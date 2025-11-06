using Mailing.Tests.CleanWriteTests.Contracts;

namespace Mailing.Tests.CleanWriteTests.Models;

public sealed class TestPostmanStatistics(int sendLimit, int currentSend)
{
    public void Write(IWritePostmanStatisticsCommand command) =>
        command.Execute(sendLimit, currentSend);
}