using Mailing.Domain.Postmans.Factories;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Postmans.UseCases.CreatePostman;

public sealed class CreatePostmanLoggingUseCase(Serilog.ILogger logger, ICreatePostmanUseCase useCase)
    : CreatePostmanUseCaseEnvelope(useCase)
{
    public override Status<IPostman> Create(PostmanConstructionContext context)
    {
        string logContext = nameof(ICreatePostmanUseCase);
        logger.Information("{Context} creating postman", logContext);
        Status<IPostman> result = base.Create(context);

        if (result.IsFailure)
        {
            logger.Error("{Context} failed. Error: {Text}", logContext, result.Error.ErrorText);
            return result.Error;
        }

        logger.Information(
            """
            {Context} created postman. Information:
            ID: {ID}
            Email: {Email}
            """,
            logContext,
            result.Value.Data.Id,
            result.Value.Data.Email
        );

        return result;
    }
}