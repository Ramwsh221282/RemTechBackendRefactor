using Shared.Infrastructure.Module.Cqrs;

namespace Users.Module.Features.UserPasswordRecovering.Interactor;

internal sealed record UserPasswordRecoverRequest(string? Email, string? Login) : ICommand;
