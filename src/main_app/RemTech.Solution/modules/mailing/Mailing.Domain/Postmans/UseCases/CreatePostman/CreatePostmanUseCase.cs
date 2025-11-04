using Mailing.Domain.Postmans.Factories;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Postmans.UseCases.CreatePostman;

public sealed class CreatePostmanUseCase(IPostmansFactory factory) : ICreatePostmanUseCase
{
    public Status<IPostman> Create(PostmanConstructionContext context) =>
        from result in factory.Construct(context)
        select result;
}