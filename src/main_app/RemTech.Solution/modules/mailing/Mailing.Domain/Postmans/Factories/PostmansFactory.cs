using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Postmans.Factories;

public sealed class PostmansFactory : IPostmansFactory
{
    public Status<IPostman> Construct(PostmanConstructionContext context)
    {
        PostmanData data = new(context.Id, context.Email, context.SmtpPassword);
        return new Postman(data);
    }
}