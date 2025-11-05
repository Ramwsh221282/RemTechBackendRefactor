using Mailing.Domain.PostedMessages;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Postmans;

public interface IPostman
{
    IPostmanData Data { get; }
    Status<IPostedMessage> Post(string email, string subject, string body);
}