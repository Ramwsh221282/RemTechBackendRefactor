using Shared.Infrastructure.Module.Cqrs;

namespace Users.Module.Features.CreateEmailConfirmation;

internal sealed record CreateEmailConfirmationCommand(Guid UserId, string InputPassword) : ICommand;
