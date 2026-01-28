namespace WebHostApplication.Modules.notifications;

/// <summary>
/// Запрос на отправку тестового сообщения.
/// </summary>
/// <param name="Recipient">Получатель тестового сообщения.</param>
public sealed record SendTestMessageRequest(string Recipient);
