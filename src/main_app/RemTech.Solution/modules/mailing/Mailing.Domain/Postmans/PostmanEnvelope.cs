using Mailing.Domain.PostedMessages;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Postmans;

public abstract class PostmanEnvelope(IPostman postman) : IPostman
{
    public IPostmanData Data { get; } = postman.Data;
    public IPostman Postman { get; init; } = postman;

    public Status<IPostedMessage> Post(string email, string subject, string body) =>
        Postman.Post(email, subject, body);
}