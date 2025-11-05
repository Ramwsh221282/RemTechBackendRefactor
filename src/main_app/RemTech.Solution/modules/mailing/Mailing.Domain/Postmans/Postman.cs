using Mailing.Domain.PostedMessages;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Postmans;

public sealed record Postman(IPostmanData Data) : IPostman
{
    public Status<IPostedMessage> Post(string email, string subject, string body)
    {
        throw new NotImplementedException();
    }
}