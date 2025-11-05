namespace Mailing.Domain.Postmans.Factories.Statistics;

public abstract class PostmanStatisticsFactoryEnvelope(IPostmanStatisticsFactory factory) : IPostmanStatisticsFactory
{
    public virtual IPostmanStatistics Construct() =>
        factory.Construct();

    public virtual IPostmanStatistics Construct(int sendLimit, int currentSend) =>
        factory.Construct(sendLimit, currentSend);
}