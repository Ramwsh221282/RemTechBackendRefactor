using Mailing.Tests.CleanWriteTests.Contracts;

namespace Mailing.Tests.CleanWriteTests.Interactor.Implementation;

public sealed class WritePostmanStatisticsInteractor(
    IWritePostmanStatisticsCommand driving,
    IWritePostmanStatisticsCommand driven)
    : IWritePostmanStatisticsInteractorCommand
{
    public void Execute(in int sendLimit, in int currentSend)
    {
        driven.Execute(in sendLimit, in currentSend);
        driving.Execute(in sendLimit, in currentSend);
    }
}