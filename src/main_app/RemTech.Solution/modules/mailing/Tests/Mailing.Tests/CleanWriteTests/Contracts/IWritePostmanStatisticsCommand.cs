namespace Mailing.Tests.CleanWriteTests.Contracts;

public interface IWritePostmanStatisticsCommand
{
    void Execute(in int sendLimit, in int currentSend);
}