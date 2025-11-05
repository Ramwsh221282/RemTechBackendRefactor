using Mailing.Domain.Postmans.Storing;

namespace Mailing.Domain.Postmans;

internal sealed class Postman(IPostmanMetadata metadata, IPostmanStatistics statistics) : IPostman
{
    public Postman(IPostmanMetadata metadata) : this(metadata, new PostmanStatistics())
    {
    }

    public void Save(IPostmansStorage postmansStorage) =>
        postmansStorage.Save((metadataStorage, statisticsStorage) =>
        {
            metadata.Save(metadataStorage);
            statistics.Save(statisticsStorage);
        });
}