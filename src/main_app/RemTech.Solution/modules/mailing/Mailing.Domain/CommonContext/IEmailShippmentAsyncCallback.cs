namespace Mailing.Domain.CommonContext;

public interface IEmailShippmentAsyncCallback : IEmailShippmentCallback
{
    Task Invoke(EmailShippmentResult result, CancellationToken ct = default);
}
