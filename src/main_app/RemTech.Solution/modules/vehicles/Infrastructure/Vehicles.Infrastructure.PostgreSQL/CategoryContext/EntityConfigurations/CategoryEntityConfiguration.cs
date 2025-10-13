using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RemTech.Infrastructure.PostgreSQL;
using Vehicles.Domain.CategoryContext;
using Vehicles.Domain.CategoryContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.CategoryContext.EntityConfigurations;

public sealed class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.Id).HasName("pk_category");

        builder
            .Property(c => c.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Value, fromDb => new CategoryId(fromDb));

        builder
            .Property(c => c.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => new CategoryName(fromDb));

        builder
            .Property(c => c.VehiclesCount)
            .HasColumnName("vehicles_count")
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => new CategoryVehiclesCount(fromDb));

        builder
            .Property(c => c.Rating)
            .HasColumnName("rating")
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => new CategoryRating(fromDb));

        builder.ConfigureVector("categories");

        builder.HasIndex(c => c.Name).IsUnique().HasDatabaseName("category_unique_name_idx");
    }
}
