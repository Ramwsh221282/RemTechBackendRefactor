using Mailing.Domain.PostmanStatistics.Factories;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.PostmanStatistics;

public interface IPostmanSendingStatistics
{
    IPostmanSendingStatisticsData Data { get; }
    bool LimitReached(out Error error);
}