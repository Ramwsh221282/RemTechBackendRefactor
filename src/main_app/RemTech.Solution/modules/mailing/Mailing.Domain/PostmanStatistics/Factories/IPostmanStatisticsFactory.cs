using RemTech.Core.Shared.Result;

namespace Mailing.Domain.PostmanStatistics.Factories;

public interface IPostmanStatisticsFactory
{
    Status<IPostmanSendingStatistics> Construct(Guid postmanId, int limit, int currentAmount);
}