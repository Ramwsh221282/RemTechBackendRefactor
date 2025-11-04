using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mailing.Adapters.Storage.Postmans;

internal sealed class DbPostmanEntityTypeConfiguration : IEntityTypeConfiguration<DbPostman>
{
    public void Configure(EntityTypeBuilder<DbPostman> builder) => DbPostman.Configure(builder);
}