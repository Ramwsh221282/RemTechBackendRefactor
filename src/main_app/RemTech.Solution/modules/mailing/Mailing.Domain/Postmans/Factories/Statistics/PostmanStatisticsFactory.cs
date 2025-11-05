namespace Mailing.Domain.Postmans.Factories.Statistics;

public sealed class PostmanStatisticsFactory : IPostmanStatisticsFactory
{
    public IPostmanStatistics Construct() => new PostmanStatistics();

    public IPostmanStatistics Construct(int sendLimit, int currentSend) =>
        new PostmanStatistics(sendLimit, currentSend);
}