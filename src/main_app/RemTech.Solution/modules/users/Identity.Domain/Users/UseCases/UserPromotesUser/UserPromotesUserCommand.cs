using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.UserPromotesUser;

public sealed record UserPromotesUserCommand(
    Guid PromoterId,
    Guid UserId,
    string PromoterPassword,
    string RoleName
) : ICommand;
