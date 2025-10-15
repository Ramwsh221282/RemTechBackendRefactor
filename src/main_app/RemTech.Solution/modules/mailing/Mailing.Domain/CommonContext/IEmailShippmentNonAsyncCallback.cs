namespace Mailing.Domain.CommonContext;

public interface IEmailShippmentNonAsyncCallback : IEmailShippmentCallback
{
    void Invoke(EmailShippmentResult result);
}
