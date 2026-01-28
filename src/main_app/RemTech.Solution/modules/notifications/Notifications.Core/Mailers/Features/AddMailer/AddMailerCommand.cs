using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.Mailers.Features.AddMailer;

/// <summary>
/// Команда добавления почтового ящика.
/// </summary>
/// <param name="SmtpPassword">Пароль SMTP для почтового ящика.</param>
/// <param name="Email">Адрес электронной почты почтового ящика.</param>
public sealed record AddMailerCommand(string SmtpPassword, string Email) : ICommand;
