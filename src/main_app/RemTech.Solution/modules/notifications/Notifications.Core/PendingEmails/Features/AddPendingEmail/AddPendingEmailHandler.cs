using Notifications.Core.PendingEmails.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.PendingEmails.Features.AddPendingEmail;

/// <summary>
/// Обработчик команды для добавления нового ожидающего email-уведомления.
/// </summary>
/// <param name="repository">Репозиторий для работы с ожидающими email-уведомлениями.</param>
public sealed class AddPendingEmailHandler(IPendingEmailNotificationsRepository repository)
	: ICommandHandler<AddPendingEmailCommand, Unit>
{
	/// <summary>
	/// Выполняет обработку команды для добавления нового ожидающего email-уведомления.
	/// </summary>
	/// <param name="command">Команда для добавления нового ожидающего email-уведомления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды.</returns>
	public async Task<Result<Unit>> Execute(AddPendingEmailCommand command, CancellationToken ct = default)
	{
		PendingEmailNotification notification = CreateNotification(command);
		await repository.Add(notification, ct);
		return Unit.Value;
	}

	private static PendingEmailNotification CreateNotification(AddPendingEmailCommand command)
	{
		return PendingEmailNotification.CreateNew(command.Recipient, command.Subject, command.Body).Value;
	}
}
