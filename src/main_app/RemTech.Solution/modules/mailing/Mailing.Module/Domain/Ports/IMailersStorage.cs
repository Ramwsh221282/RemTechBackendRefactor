using Mailing.Module.Domain.Models;

namespace Mailing.Module.Domain.Ports;

internal interface IMailersStorage<in TSearchCriteriaSupport> where TSearchCriteriaSupport : IMailersSearchCriteria
{
    Task Add(IMailer postman, CancellationToken ct = default);
    Task Remove(IMailer postman, CancellationToken ct = default);
    Task<IMailer> Find(TSearchCriteriaSupport criteria, CancellationToken ct = default);
}