namespace Mailing.Module.Contracts;

public interface IEmailSendersSource
{
    Task<IEmailSender> Get(string name, CancellationToken ct = default);
    Task<bool> Save(IEmailSender sender, CancellationToken ct = default);
    Task<bool> Update(IEmailSender sender, CancellationToken ct = default);
    Task<bool> Remove(IEmailSender sender, CancellationToken ct = default);

    Task<IEnumerable<IEmailSender>> ReadAll(CancellationToken ct = default);
}
