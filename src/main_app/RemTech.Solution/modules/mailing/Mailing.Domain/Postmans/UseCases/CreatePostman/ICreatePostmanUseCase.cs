using Mailing.Domain.Postmans.Factories;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Postmans.UseCases.CreatePostman;

public interface ICreatePostmanUseCase
{
    Status<IPostman> Create(PostmanConstructionContext context);
}