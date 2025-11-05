namespace Mailing.Domain.Postmans.Factories;

public interface IPostmansFactory
{
    IPostman Construct(Guid id, string email, string password, int sendLimit, int currentSend);
    IPostman Construct(string email, string password, int sendLimit, int currentSend);
    IPostman Construct(string email, string password);
    IPostman Construct(Guid id, string email, string password);
}