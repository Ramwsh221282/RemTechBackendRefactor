using FluentValidation;

namespace Identity.Domain.Accounts.Features.Refresh;

public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Token not provided.");
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh token not provided.");
    }
}
