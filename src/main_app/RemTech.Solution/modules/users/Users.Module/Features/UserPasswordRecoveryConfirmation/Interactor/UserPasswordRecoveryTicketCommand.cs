using Shared.Infrastructure.Module.Cqrs;

namespace Users.Module.Features.UserPasswordRecoveryConfirmation.Interactor;

internal sealed record UserPasswordRecoveryTicketCommand(string Key) : ICommand;
