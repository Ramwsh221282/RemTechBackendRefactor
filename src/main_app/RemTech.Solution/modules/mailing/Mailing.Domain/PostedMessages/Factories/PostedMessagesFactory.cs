using RemTech.Core.Shared.Result;

namespace Mailing.Domain.PostedMessages.Factories;

public sealed class PostedMessagesFactory : IPostedMessagesFactory
{
    public Status<IPostedMessagesFactory> Construct(string email, string subject, string body)
    {
        throw new NotImplementedException();
    }
}