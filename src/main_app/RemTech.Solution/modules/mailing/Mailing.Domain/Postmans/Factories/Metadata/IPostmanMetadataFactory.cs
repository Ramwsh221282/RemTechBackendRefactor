namespace Mailing.Domain.Postmans.Factories.Metadata;

public interface IPostmanMetadataFactory
{
    IPostmanMetadata Construct(Guid id, string email, string password);
    IPostmanMetadata Construct(string email, string password);
}