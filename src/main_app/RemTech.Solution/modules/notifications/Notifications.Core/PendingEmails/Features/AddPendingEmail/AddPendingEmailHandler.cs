using Notifications.Core.PendingEmails.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.PendingEmails.Features.AddPendingEmail;

public sealed class AddPendingEmailHandler(IPendingEmailNotificationsRepository repository)
	: ICommandHandler<AddPendingEmailCommand, Unit>
{
	public async Task<Result<Unit>> Execute(
		AddPendingEmailCommand command,
		CancellationToken ct = new CancellationToken()
	)
	{
		PendingEmailNotification notification = CreateNotification(command);
		await repository.Add(notification, ct);
		return Unit.Value;
	}

	private PendingEmailNotification CreateNotification(AddPendingEmailCommand command) =>
		PendingEmailNotification.CreateNew(command.Recipient, command.Subject, command.Body).Value;
}
