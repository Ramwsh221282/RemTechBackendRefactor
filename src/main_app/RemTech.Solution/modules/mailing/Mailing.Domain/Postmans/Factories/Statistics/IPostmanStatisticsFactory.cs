namespace Mailing.Domain.Postmans.Factories.Statistics;

public interface IPostmanStatisticsFactory
{
    IPostmanStatistics Construct();
    IPostmanStatistics Construct(int sendLimit, int currentSend);
}