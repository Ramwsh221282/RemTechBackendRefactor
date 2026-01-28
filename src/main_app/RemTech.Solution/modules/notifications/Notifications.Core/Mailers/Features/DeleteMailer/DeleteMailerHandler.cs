using Notifications.Core.Mailers.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace Notifications.Core.Mailers.Features.DeleteMailer;

/// <summary>
/// Обработчик команды удаления почтового ящика.
/// </summary>
/// <param name="repository">Репозиторий для работы с почтовыми ящиками.</param>
[TransactionalHandler]
public sealed class DeleteMailerHandler(IMailersRepository repository) : ICommandHandler<DeleteMailerCommand, Guid>
{
	/// <summary>
	/// Выполняет команду удаления почтового ящика.
	/// </summary>
	/// <param name="command">Команда удаления почтового ящика.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Идентификатор удаленного почтового ящика.</returns>
	public async Task<Result<Guid>> Execute(DeleteMailerCommand command, CancellationToken ct = default)
	{
		Result<Mailer> mailer = await GetRequiredMailer(command, ct);
		if (mailer.IsFailure)
			return mailer.Error;

		await repository.Delete(mailer.Value, ct);
		return mailer.Value.Id.Value;
	}

	private Task<Result<Mailer>> GetRequiredMailer(DeleteMailerCommand command, CancellationToken ct)
	{
		MailersSpecification specification = new MailersSpecification().WithId(command.Id).WithLockRequired();
		return repository.Get(specification, ct);
	}
}
