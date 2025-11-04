namespace Mailing.Domain.PostmanStatistics;

public interface IDbPostmanData
{
    Guid PostmanId { get; }
    int Limit { get; }
    int CurrentSent { get; }
}