namespace Mailing.Domain.Postmans.Storing;

public interface IPostmanMetadataStorage
{
    void Save(Guid id, string email, string password);
}