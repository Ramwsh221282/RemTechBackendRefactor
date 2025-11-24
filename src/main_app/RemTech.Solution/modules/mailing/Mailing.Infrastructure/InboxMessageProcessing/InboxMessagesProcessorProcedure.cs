using Mailing.Core.Inbox;
using Mailing.Core.Mailers;

namespace Mailing.Infrastructure.InboxMessageProcessing;

public sealed class InboxMessagesProcessorProcedure : InboxMessagesProcessorProtocol
{
    public async Task<InboxMessagesProcessorResult> ProcessAsync(
        Func<CancellationToken, Task<Mailer?>> mailerSource,
        InboxMessagesProcessorProcedureDependencies dependencies, 
        CancellationToken ct)
    {
        Mailer? mailer = await mailerSource(ct);
        
        if (mailer is null)
            return new InboxMessagesProcessorResult(
                "Обработка почтовых сообщений невозможна. Нет доступных почтовых отправителей");
            
        Mailer resolved = mailer with { Domain = mailer.Domain.WithResolvedService() };
        Mailer decrypted = await dependencies.Execute(async (d, token) =>
            await resolved.Decrypted(d.DecryptMailer, token), ct);
        
        List<InboxMessage> processed = [];
        IAsyncEnumerable<InboxMessage> pending = dependencies.Execute((d, token) =>
            d.GetInboxMessages.GetPendingMessages(token), ct);
        
        await foreach (InboxMessage message in pending)
        {
            await dependencies.Execute(async (d, token) =>
                await decrypted.SendMessage(message, d.DeliverMessage, token), ct);
            
            processed.Add(message);
        }
                
        Mailer encrypted = await dependencies.Execute(async (d, token) =>
            await decrypted.Encrypted(d.EncryptMailer, token), ct);
        
        await dependencies.Execute(async (d, token) =>
            await d.RemoveManyInboxMessage.Remove(processed, token), ct);
        
        await dependencies.Execute(async (d, token) =>
            await encrypted.Save(d.SaveMailer, token), ct);
        
        return new InboxMessagesProcessorResult($"Обработано: {processed.Count} сообщений.");
    }
}