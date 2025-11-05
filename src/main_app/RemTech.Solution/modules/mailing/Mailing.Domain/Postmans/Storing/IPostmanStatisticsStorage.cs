namespace Mailing.Domain.Postmans.Storing;

public interface IPostmanStatisticsStorage
{
    void Save(int sendLimit, int currentSend);
}