using Mailing.Domain.Postmans.Storing;

namespace Mailing.Domain.Postmans;

public interface IPostmanMetadata
{
    void Save(IPostmanMetadataStorage storage);
}