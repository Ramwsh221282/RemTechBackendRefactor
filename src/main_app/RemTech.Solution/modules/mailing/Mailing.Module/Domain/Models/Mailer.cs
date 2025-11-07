using Mailing.Module.Domain.Models.ValueObjects;

namespace Mailing.Module.Domain.Models;

internal sealed class Mailer(Metadata metadata, Statistics statistics) : IMailer
{
    private readonly Metadata _metadata = metadata;
    private readonly Statistics _statistics = statistics;

    public Task Save<TSearchCriteria>(
        IMailersStorage<TSearchCriteria> mailersStorage,
        CancellationToken ct = default)
        where TSearchCriteria : IMailersSearchCriteria
        =>
            mailersStorage.Add(this, ct);

    public bool EqualById(Mailer mailer) =>
        _metadata.EqualById(mailer._metadata);

    public Mailer(Metadata metadata) : this(metadata, new Statistics())
    {
    }
}