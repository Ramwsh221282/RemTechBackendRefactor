using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mailing.Adapters.Storage.PostmanStatistics;

internal sealed class DbPostmanStatisticsEntityTypeConfiguration : IEntityTypeConfiguration<DbPostmanStatistics>
{
    public void Configure(EntityTypeBuilder<DbPostmanStatistics> builder) => DbPostmanStatistics.Configure(builder);
}