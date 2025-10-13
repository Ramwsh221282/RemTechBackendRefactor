using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RemTech.Infrastructure.PostgreSQL;
using Vehicles.Domain.BrandContext;
using Vehicles.Domain.BrandContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.BrandContext.EntityConfigurations;

public sealed class BrandEntityConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("brands");

        builder.HasKey(b => b.Id).HasName("pk_brands");

        builder
            .Property(b => b.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Id, fromDb => new BrandId(fromDb));

        builder
            .Property(b => b.Name)
            .HasColumnName("name")
            .HasMaxLength(BrandName.MaxLength)
            .HasConversion(toDb => toDb.Name, fromDb => new BrandName(fromDb))
            .IsRequired();

        builder
            .Property(b => b.Rating)
            .HasColumnName("rating")
            .HasConversion(toDb => toDb.Value, fromDb => new BrandRating(fromDb))
            .IsRequired();

        builder
            .Property(b => b.VehiclesCount)
            .HasColumnName("vehicles_count")
            .HasConversion(toDb => toDb.Value, fromDb => new BrandVehiclesCount(fromDb))
            .IsRequired();

        builder.ConfigureVector("brands");

        builder.HasIndex(b => b.Name).IsUnique().HasDatabaseName("brands_unique_name_idx");
    }
}
