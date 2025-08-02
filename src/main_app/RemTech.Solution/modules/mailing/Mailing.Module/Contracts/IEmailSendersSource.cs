namespace Mailing.Module.Contracts;

public interface IEmailSendersSource
{
    Task<IEmailSender> Get(string email, CancellationToken ct = default);
    Task<bool> Save(IEmailSender sender, CancellationToken ct = default);
    Task<bool> Remove(IEmailSender sender, CancellationToken ct = default);
    Task<IEnumerable<IEmailSender>> ReadAll(CancellationToken ct = default);
}
