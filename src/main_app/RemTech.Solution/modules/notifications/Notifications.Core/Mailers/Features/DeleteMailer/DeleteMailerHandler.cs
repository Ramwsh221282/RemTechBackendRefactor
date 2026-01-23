using Notifications.Core.Mailers.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace Notifications.Core.Mailers.Features.DeleteMailer;

[TransactionalHandler]
public sealed class DeleteMailerHandler(IMailersRepository repository) : ICommandHandler<DeleteMailerCommand, Guid>
{
	public async Task<Result<Guid>> Execute(DeleteMailerCommand command, CancellationToken ct = default)
	{
		Result<Mailer> mailer = await GetRequiredMailer(command, ct);
		if (mailer.IsFailure)
			return mailer.Error;

		await repository.Delete(mailer.Value, ct);
		return mailer.Value.Id.Value;
	}

	private async Task<Result<Mailer>> GetRequiredMailer(DeleteMailerCommand command, CancellationToken ct)
	{
		MailersSpecification specification = new MailersSpecification().WithId(command.Id).WithLockRequired();
		return await repository.Get(specification, ct);
	}
}
