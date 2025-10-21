using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.UserRegistration;

public sealed record UserRegistrationCommand(
    string UserLogin,
    string UserEmail,
    string UserPassword
) : ICommand;
