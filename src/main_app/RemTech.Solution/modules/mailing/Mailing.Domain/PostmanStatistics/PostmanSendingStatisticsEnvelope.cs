using Mailing.Domain.PostmanStatistics.Factories;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.PostmanStatistics;

public abstract record PostmanSendingStatisticsEnvelope(IPostmanSendingStatistics Statistics)
    : IPostmanSendingStatistics
{
    public IPostmanSendingStatisticsData Data { get; } = Statistics.Data;

    public bool LimitReached(out Error error) => Statistics.LimitReached(out error);
}