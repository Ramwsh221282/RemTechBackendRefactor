namespace Mailing.Domain.PostmanStatistics;

public sealed record PostmanSendingStatisticsData(
    Guid PostmanId,
    int Limit,
    int CurrentAmount)
    : IPostmanSendingStatisticsData;