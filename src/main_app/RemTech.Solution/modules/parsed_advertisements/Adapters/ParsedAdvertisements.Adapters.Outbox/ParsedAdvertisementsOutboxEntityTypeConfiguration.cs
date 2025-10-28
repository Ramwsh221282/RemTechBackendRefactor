using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Outbox;

namespace ParsedAdvertisements.Adapters.Outbox;

public sealed class
    ParsedAdvertisementsOutboxEntityTypeConfiguration : IEntityTypeConfiguration<ParsedAdvertisementsOutboxMessage>
{
    public void Configure(EntityTypeBuilder<ParsedAdvertisementsOutboxMessage> builder)
    {
        builder.ToTable("outbox");
        builder.HasKey(x => x.Id).HasName("pk_outbox");
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.Content).HasColumnName("content").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.Created).HasColumnName("created").IsRequired();
        builder.Property(x => x.LastError).HasColumnName("last_error").IsRequired(false);
        builder.Property(x => x.Retries).HasColumnName("retries").IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired().HasMaxLength(100);
    }
}