using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.ConfirmPasswordReset;

/// <summary>
/// Команда для подтверждения сброса пароля пользователя.
/// </summary>
/// <param name="AccountId">Идентификатор аккаунта пользователя.</param>
/// <param name="TicketId">Идентификатор тикета сброса пароля.</param>
/// <param name="NewPassword">Новый пароль пользователя.</param>
public sealed record ConfirmResetPasswordCommand(Guid AccountId, Guid TicketId, string NewPassword) : ICommand;
