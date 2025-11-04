using Mailing.Domain.Postmans.Factories;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Postmans.UseCases.CreatePostman;

public abstract class CreatePostmanUseCaseEnvelope(ICreatePostmanUseCase useCase) : ICreatePostmanUseCase
{
    public virtual Status<IPostman> Create(PostmanConstructionContext context) => useCase.Create(context);
}