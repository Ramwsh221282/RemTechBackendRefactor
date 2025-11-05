using RemTech.Core.Shared.Result;

namespace Mailing.Domain.PostmanStatistics.Factories;

public sealed class PostmanStatisticsFactory : IPostmanStatisticsFactory
{
    public Status<IPostmanSendingStatistics> Construct(Guid postmanId, int limit, int currentAmount)
    {
        PostmanSendingStatisticsData data = new(postmanId, limit, currentAmount);
        return new PostmanSendingStatistics(data);
    }
}