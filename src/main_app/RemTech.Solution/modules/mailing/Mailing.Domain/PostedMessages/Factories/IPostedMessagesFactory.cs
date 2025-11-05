using RemTech.Core.Shared.Result;

namespace Mailing.Domain.PostedMessages.Factories;

public interface IPostedMessagesFactory
{
    Status<IPostedMessagesFactory> Construct(string email, string subject, string body);
}

public abstract class PostedMessagesFactoryEnvelope(IPostedMessagesFactory origin) : IPostedMessagesFactory
{
    public Status<IPostedMessagesFactory> Construct(string email, string subject, string body)
    {
        return origin.Construct(email, subject, body);
    }
}

public sealed class PostedMessagesFactory : IPostedMessagesFactory
{
    public Status<IPostedMessagesFactory> Construct(string email, string subject, string body)
    {
        throw new NotImplementedException();
    }
}