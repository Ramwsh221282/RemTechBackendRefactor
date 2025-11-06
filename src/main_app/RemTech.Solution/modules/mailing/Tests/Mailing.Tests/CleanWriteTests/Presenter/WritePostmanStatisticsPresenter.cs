using Mailing.Tests.CleanWriteTests.Presenter.Implementation;

namespace Mailing.Tests.CleanWriteTests.Presenter;

public sealed class WritePostmanStatisticsPresenter(PostmanDto postmanDto) : IWritePostmanStatisticsPresenter
{
    public void Execute(in int sendLimit, in int currentSend)
    {
        postmanDto.Limit = sendLimit;
        postmanDto.CurrentSend = currentSend;
    }
}