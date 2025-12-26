using Cleaners.Domain.Cleaners.Ports.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cleaners.Adapter.Storage.Configurations;

public sealed class CleanersOutboxMessageEntityTypeConfiguration
    : IEntityTypeConfiguration<CleanerOutboxMessage>
{
    public void Configure(EntityTypeBuilder<CleanerOutboxMessage> builder)
    {
        builder.ToTable("outbox");
        builder.HasKey(x => x.Id).HasName("pk_outbox");
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").HasMaxLength(255).IsRequired();
        builder
            .Property(x => x.Content)
            .HasColumnType("jsonb")
            .HasColumnName("content")
            .IsRequired();
        builder.Property(x => x.Created).HasColumnName("created").IsRequired();
        builder.Property(x => x.Processed).HasColumnName("processed").IsRequired(false);
        builder.Property(x => x.ProcessedAttempts).HasColumnName("processed_attempts").IsRequired();
        builder.Property(x => x.LastError).HasColumnName("last_error").IsRequired(false);
    }
}
