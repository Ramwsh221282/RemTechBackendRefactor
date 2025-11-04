using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Postmans.Factories;

public abstract class PostmansFactoryEnvelope(IPostmansFactory factory) : IPostmansFactory
{
    public Status<IPostman> Construct(PostmanConstructionContext context)
    {
        return factory.Construct(context);
    }
}