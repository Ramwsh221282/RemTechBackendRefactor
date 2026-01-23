using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Notifications.Core.Mailers.Contracts;

public interface IMailersRepository
{
	public Task Add(Mailer mailer, CancellationToken ct = default);
	public Task<Result<Mailer>> Get(MailersSpecification specification, CancellationToken ct = default);
	public Task<Mailer[]> GetMany(MailersSpecification specification, CancellationToken ct = default);
	public Task<bool> Exists(MailersSpecification specification, CancellationToken ct = default);
	public Task Delete(Mailer mailer, CancellationToken ct = default);
}
