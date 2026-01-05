using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Notifications.Core.Mailers.Contracts;

public interface IMailersRepository
{
    Task Add(Mailer mailer, CancellationToken ct = default);
    Task<Result<Mailer>> Get(MailersSpecification specification, CancellationToken ct = default);
    Task<Mailer[]> GetMany(MailersSpecification specification, CancellationToken ct = default);
    Task<bool> Exists(MailersSpecification specification, CancellationToken ct = default);
}