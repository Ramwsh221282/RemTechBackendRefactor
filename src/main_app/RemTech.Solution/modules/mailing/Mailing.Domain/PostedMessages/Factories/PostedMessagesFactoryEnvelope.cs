using RemTech.Core.Shared.Result;

namespace Mailing.Domain.PostedMessages.Factories;

public abstract class PostedMessagesFactoryEnvelope(IPostedMessagesFactory origin) : IPostedMessagesFactory
{
    public Status<IPostedMessagesFactory> Construct(string email, string subject, string body)
    {
        return origin.Construct(email, subject, body);
    }
}