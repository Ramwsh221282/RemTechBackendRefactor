using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.UserAdminVerification.Input;

public sealed record UserAdminVerificationCommand(Guid TokenId) : ICommand;
