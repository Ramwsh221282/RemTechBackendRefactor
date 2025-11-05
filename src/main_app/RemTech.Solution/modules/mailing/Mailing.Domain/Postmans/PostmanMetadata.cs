using Mailing.Domain.General;
using Mailing.Domain.Postmans.Storing;

namespace Mailing.Domain.Postmans;

internal sealed class PostmanMetadata(Guid id, string email, string password) : IPostmanMetadata
{
    public PostmanMetadata(string email, string password) : this(Guid.NewGuid(), email, password)
    {
    }

    public void Save(IPostmanMetadataStorage storage) =>
        storage.Save(id, email, password);
}