using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.UserTokenVerification.Input;

public sealed record UserTokenVerificationCommand(Guid TokenId) : ICommand;
