using Mailing.Domain.PostedMessages;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Postmans;

public abstract class PostmanEnvelope(IPostman Postman) : IPostman
{
    public IPostmanData Data { get; } = Postman.Data;
    public IPostman Postman { get; init; } = Postman;

    public Status<IPostedMessage> Post(string email, string subject, string body) =>
        Postman.Post(email, subject, body);

    public void Deconstruct(out IPostman Postman)
    {
        Postman = this.Postman;
    }
}