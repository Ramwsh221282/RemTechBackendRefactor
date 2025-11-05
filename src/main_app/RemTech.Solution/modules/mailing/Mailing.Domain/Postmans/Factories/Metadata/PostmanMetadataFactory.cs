namespace Mailing.Domain.Postmans.Factories.Metadata;

public sealed class PostmanMetadataFactory : IPostmanMetadataFactory
{
    public IPostmanMetadata Construct(Guid id, string email, string password) =>
        new PostmanMetadata(id, email, password);

    public IPostmanMetadata Construct(string email, string password) =>
        new PostmanMetadata(email, password);
}