using RemTech.Core.Shared.Result;

namespace Mailing.Domain.PostmanStatistics.Factories;

public sealed class PostmanStatisticsFactory : IPostmanStatisticsFactory
{
    public Status<IPostmanSendingStatistics> Construct(Guid postmanId, int limit, int currentAmount)
    {
        return new PostmanSendingStatistics(postmanId, limit, currentAmount);
    }
}