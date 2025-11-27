using Mailing.Core.Mailers.Protocols;
using Mailing.Infrastructure.InboxMessageProcessing.Protocols;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace Mailing.Infrastructure.InboxMessageProcessing;

public sealed record InboxMessagesProcessorProcedureDependencies
(
    NpgSqlSession Session,
    GetMailerProtocol GetMailer,
    GetPendingInboxMessagesProtocol GetInboxMessages,
    RemoveManyInboxMessagesProtocol RemoveManyInboxMessage,
    EncryptMailerSmtpPasswordProtocol EncryptMailer,
    DecryptMailerSmtpPasswordProtocol DecryptMailer,
    MessageDeliveryProtocol DeliverMessage,
    SaveMailerProtocol SaveMailer,
    MarkProcessedMessagesProtocol MarkProcessed,
    Serilog.ILogger Logger
) : IDisposable, IAsyncDisposable
{
    public void Dispose()
    {
        Session.Dispose();
    }

    public async Task<U> Execute<U>(
        Func<InboxMessagesProcessorProcedureDependencies, CancellationToken, Task<U>> func, 
        CancellationToken ct)
    {
        U result = await func(this, ct);
        return result;
    }

    public void Execute(Action<InboxMessagesProcessorProcedureDependencies> func)
    {
        func(this);
    }
    
    public U Execute<U>(
        Func<InboxMessagesProcessorProcedureDependencies, CancellationToken, U> func,
        CancellationToken ct)
    {
        return func(this, ct);
    }
    
    public async Task Execute(
        Func<InboxMessagesProcessorProcedureDependencies, CancellationToken, Task> func,
        CancellationToken ct)
    {
        await func(this, ct);
    }
    
    public async ValueTask DisposeAsync()
    {
        await Session.DisposeAsync();
    }
}