using Mailing.Domain.PostmanStatistics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mailing.Adapters.Storage.PostmanStatistics;

internal sealed record DbPostmanStatistics(IPostmanSendingStatistics statistics)
    : PostmanSendingStatisticsEnvelope(statistics)
{
    internal Guid Id { get; } = statistics.Data.PostmanId;
    internal int Limit { get; } = statistics.Data.Limit;
    internal int CurrentSet { get; } = statistics.Data.CurrentSent;

    internal static void Configure(EntityTypeBuilder<DbPostmanStatistics> builder)
    {
        builder.ToTable("postman_statistics");
        builder.HasKey(x => x.Id).HasName("pk_postman_statistics");
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.CurrentSet).HasColumnName("current_set").IsRequired();
        builder.Property(x => x.Limit).HasColumnName("limit").IsRequired();
    }
}