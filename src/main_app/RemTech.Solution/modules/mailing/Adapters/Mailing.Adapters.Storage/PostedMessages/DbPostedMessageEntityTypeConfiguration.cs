using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mailing.Adapters.Storage.PostedMessages;

internal sealed class DbPostedMessageEntityTypeConfiguration : IEntityTypeConfiguration<DbPostedMessage>
{
    public void Configure(EntityTypeBuilder<DbPostedMessage> builder) => DbPostedMessage.Configure(builder);
}