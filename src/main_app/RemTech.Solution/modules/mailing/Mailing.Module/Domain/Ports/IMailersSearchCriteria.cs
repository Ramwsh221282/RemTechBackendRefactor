using Mailing.Module.Domain.Models;

namespace Mailing.Module.Domain.Ports;

internal interface IMailersSearchCriteria
{
    Task<IMailer> Find(CancellationToken ct = default);
}