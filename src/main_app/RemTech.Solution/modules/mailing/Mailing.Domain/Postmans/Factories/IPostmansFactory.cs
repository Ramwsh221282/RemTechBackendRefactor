using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Postmans.Factories;

public interface IPostmansFactory
{
    Status<IPostman> Construct(PostmanConstructionContext context);
}