using RemTech.Core.Shared.Result;

namespace Mailing.Domain.PostmanStatistics.Factories;

public abstract class PostmanStatisticsFactoryEnvelope(IPostmanStatisticsFactory origin) : IPostmanStatisticsFactory
{
    public Status<IPostmanSendingStatistics> Construct(Guid postmanId, int limit, int currentAmount)
    {
        return origin.Construct(postmanId, limit, currentAmount);
    }
}