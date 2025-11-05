using Mailing.Domain.Postmans.Storing;

namespace Mailing.Domain.Postmans;

public interface IPostmanStatistics
{
    void Save(IPostmanStatisticsStorage storage);
}