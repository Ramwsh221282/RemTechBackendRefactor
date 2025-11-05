namespace Mailing.Domain.PostmanStatistics;

public interface IPostmanSendingStatisticsData
{
    public Guid PostmanId { get; }
    public int Limit { get; }
    public int CurrentAmount { get; }
}