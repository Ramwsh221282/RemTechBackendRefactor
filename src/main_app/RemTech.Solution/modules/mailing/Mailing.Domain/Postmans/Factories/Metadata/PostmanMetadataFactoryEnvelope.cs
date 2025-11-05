namespace Mailing.Domain.Postmans.Factories.Metadata;

public abstract class PostmanMetadataFactoryEnvelope(IPostmanMetadataFactory factory) : IPostmanMetadataFactory
{
    public virtual IPostmanMetadata Construct(Guid id, string email, string password) =>
        factory.Construct(id, email, password);

    public virtual IPostmanMetadata Construct(string email, string password) =>
        factory.Construct(email, password);
}