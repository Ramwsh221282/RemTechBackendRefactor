using Mailing.Domain.CommonContext.ValueObjects;
using Mailing.Domain.EmailShipperContext;
using Mailing.Domain.EmailShipperContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mailing.Infrastructure.PostgreSQL.EmailShipperContext;

public sealed class EmailShipperEntityConfiguration : IEntityTypeConfiguration<EmailShipper>
{
    public void Configure(EntityTypeBuilder<EmailShipper> builder)
    {
        builder.ToTable("shippers");

        builder.HasKey(x => x.Id).HasName("pk_shippers");

        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Value, fromDb => EmailShipperId.Create(fromDb));

        builder
            .Property(x => x.Key)
            .HasColumnName("key")
            .HasConversion(toDb => toDb.Value, fromDb => EmailShipperKey.Create(fromDb));

        builder.ComplexProperty(
            x => x.Address,
            ab =>
            {
                ab.Property(a => a.Address)
                    .HasColumnName("email")
                    .IsRequired()
                    .HasConversion(toDb => toDb.Value, fromDb => EmailAddress.Create(fromDb));

                ab.Property(a => a.SmtpHost).HasColumnName("smtp_host").IsRequired();
            }
        );

        builder.HasIndex("email").HasMethod("unique").HasDatabaseName("idx_shippers_unique");
    }
}
