namespace Mailing.Module.Domain.Models;

internal interface IMailer
{
    Task Save<TSearchCriteria>(IMailersStorage<TSearchCriteria> mailersStorage, CancellationToken ct = default)
        where TSearchCriteria : IMailersSearchCriteria;
}