namespace Mailing.Tests.CleanWriteTests.Models;

public sealed class TestPostmanStatistics(int sendLimit, int currentSend)
{
    public PostmanSnapshot Supply(PostmanSnapshot snapshot) =>
        snapshot with { LimitSend = sendLimit, CurrentSend = currentSend };

    public TestPostmanStatistics() : this(0, 0)
    {
    }
}