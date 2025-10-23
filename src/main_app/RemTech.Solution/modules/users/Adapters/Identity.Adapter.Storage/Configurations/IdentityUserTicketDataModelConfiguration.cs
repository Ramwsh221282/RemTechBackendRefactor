using Identity.Adapter.Storage.DataModels;
using Identity.Domain.Users.Entities.Tickets.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Adapter.Storage.Configurations;

public sealed class IdentityUserTicketDataModelConfiguration
    : IEntityTypeConfiguration<IdentityUserTicketDataModel>
{
    public void Configure(EntityTypeBuilder<IdentityUserTicketDataModel> builder)
    {
        builder.ToTable("tickets");
        builder.HasKey(t => t.Id).HasName("pk_tickets");
        builder.Property(t => t.Id).HasColumnName("id");
        builder.Property(t => t.UserId).HasColumnName("user_id").IsRequired();
        builder
            .Property(t => t.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasMaxLength(UserTicketType.MaxLength);
        builder.Property(t => t.Created).HasColumnName("created").IsRequired();
        builder.Property(t => t.Expired).HasColumnName("expired").IsRequired();
        builder.Property(t => t.Deleted).HasColumnName("deleted").IsRequired(false);
    }
}
