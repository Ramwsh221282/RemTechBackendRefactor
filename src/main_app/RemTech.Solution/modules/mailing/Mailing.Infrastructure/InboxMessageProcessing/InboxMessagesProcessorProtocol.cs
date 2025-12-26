using Mailing.Core.Mailers;

namespace Mailing.Infrastructure.InboxMessageProcessing;

public interface InboxMessagesProcessorProtocol
{
    Task<InboxMessagesProcessorResult> ProcessAsync(
        Func<CancellationToken, Task<Mailer?>> mailerSource,
        InboxMessagesProcessorProcedureDependencies dependencies, CancellationToken ct);
}