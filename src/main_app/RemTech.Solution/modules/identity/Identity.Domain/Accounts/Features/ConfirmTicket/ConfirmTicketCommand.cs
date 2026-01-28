using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.ConfirmTicket;

/// <summary>
/// Команда для подтверждения тикета пользователя.
/// </summary>
/// <param name="AccountId">Идентификатор аккаунта пользователя.</param>
/// <param name="TicketId">Идентификатор тикета пользователя.</param>
public sealed record ConfirmTicketCommand(Guid AccountId, Guid TicketId) : ICommand;
