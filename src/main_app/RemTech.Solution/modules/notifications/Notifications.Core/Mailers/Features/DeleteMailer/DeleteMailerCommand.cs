using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.Mailers.Features.DeleteMailer;

/// <summary>
/// Команда удаления почтового ящика.
/// </summary>
/// <param name="Id">Идентификатор почтового ящика.</param>
public sealed record DeleteMailerCommand(Guid Id) : ICommand;
