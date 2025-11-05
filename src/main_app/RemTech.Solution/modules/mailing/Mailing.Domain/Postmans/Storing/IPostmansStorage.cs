namespace Mailing.Domain.Postmans.Storing;

public interface IPostmansStorage : IPostmanStatisticsStorage, IPostmanMetadataStorage
{
    void Save(Action<IPostmanMetadataStorage, IPostmanStatisticsStorage> saveDelegate);
}