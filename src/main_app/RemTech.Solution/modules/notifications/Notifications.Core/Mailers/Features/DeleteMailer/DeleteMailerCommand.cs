using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.Mailers.Features.DeleteMailer;

public sealed record DeleteMailerCommand(Guid Id) : ICommand;