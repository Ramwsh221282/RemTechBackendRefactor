namespace WebHostApplication.Modules.notifications;

/// <summary>
/// Запрос на добавление почтового ящика для отправки уведомлений.
/// </summary>
/// <param name="SmtpPassword">Пароль SMTP для почтового ящика.</param>
/// <param name="Email">Адрес электронной почты почтового ящика.</param>
public sealed record AddMailerRequest(string SmtpPassword, string Email);
