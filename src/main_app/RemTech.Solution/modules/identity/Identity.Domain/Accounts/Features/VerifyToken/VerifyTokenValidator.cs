using FluentValidation;

namespace Identity.Domain.Accounts.Features.VerifyToken;

public sealed class VerifyTokenValidator : AbstractValidator<VerifyTokenCommand>
{
    public VerifyTokenValidator()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage("Токен не предоставлен.");
    }
}
