namespace WebHostApplication.Modules.Notifications;

/// <summary>
/// Запрос на изменение почтового ящика для отправки уведомлений.
/// </summary>
/// <param name="SmtpPassword">Пароль SMTP для почтового ящика.</param>
/// <param name="Email">Адрес электронной почты почтового ящика.</param>
public sealed record ChangeMailerRequest(string SmtpPassword, string Email);
