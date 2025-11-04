using Mailing.Domain.Emails;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Postmans.Factories;

public sealed class PostmansValidatingFactory(IPostmansFactory factory) : PostmansFactoryEnvelope(factory)
{
    public Status<IPostman> BuildFrom(PostmanConstructionContext context)
    {
        if (context.Id == Guid.Empty)
            return Error.Validation("Идентификатор postman был пустым.");
        if (string.IsNullOrEmpty(context.SmtpPassword))
            return Error.Validation("Postman не может быть создан без SMTP ключа.");
        if (string.IsNullOrEmpty(context.Email))
            return Error.Validation("Почта postman не была указана.");
        if (!new EmailString(context.Email).OfCorrectFormat(out Error error))
            return error;
        return Construct(context);
    }
}