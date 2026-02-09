using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.PendingEmails.Features.AddPendingEmail;

/// <summary>
/// Команда для добавления нового ожидающего email-уведомления.
/// </summary>
/// <param name="Recipient">Адресат email-уведомления.</param>
/// <param name="Subject">Тема email-уведомления.</param>
/// <param name="Body">Содержимое email-уведомления.</param>
public sealed record AddPendingEmailCommand(string Recipient, string Subject, string Body) : ICommand;
