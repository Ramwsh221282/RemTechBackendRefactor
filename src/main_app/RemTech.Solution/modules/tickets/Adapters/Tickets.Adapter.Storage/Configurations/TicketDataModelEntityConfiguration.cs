using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tickets.Adapter.Storage.DataModels;

namespace Tickets.Adapter.Storage.Configurations;

public sealed class TicketDataModelEntityConfiguration : IEntityTypeConfiguration<TicketDataModel>
{
    public void Configure(EntityTypeBuilder<TicketDataModel> builder)
    {
        builder.ToTable("tickets");

        builder.HasKey(x => x.Id).HasName("pk_tickets");

        builder.Property(x => x.Id).HasColumnName("id").IsRequired();

        builder.Property(x => x.Created).HasColumnName("created").IsRequired();

        builder.Property(x => x.Deleted).HasColumnName("deleted").IsRequired(false);

        builder
            .Property(x => x.Content)
            .HasColumnName("content")
            .IsRequired()
            .HasColumnType("jsonb");
    }
}
