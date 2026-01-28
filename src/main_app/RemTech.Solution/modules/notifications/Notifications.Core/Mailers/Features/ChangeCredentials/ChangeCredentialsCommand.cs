using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.Mailers.Features.ChangeCredentials;

/// <summary>
/// Команда изменения учетных данных почтового ящика.
/// </summary>
/// <param name="Id">Идентификатор почтового ящика.</param>
/// <param name="SmtpPassword">Пароль SMTP для почтового ящика.</param>
/// <param name="Email">Адрес электронной почты почтового ящика.</param>
public sealed record ChangeCredentialsCommand(Guid Id, string SmtpPassword, string Email) : ICommand;
