using RemTech.Core.Shared.Result;

namespace Mailing.Domain.PostedMessages.Factories;

public interface IPostedMessagesFactory
{
    Status<IPostedMessagesFactory> Construct(string email, string subject, string body);
}