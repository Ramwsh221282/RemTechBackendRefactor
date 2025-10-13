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
            .HasConversion(toDb => toDb.Value, fromDb => CategoryId.Create(fromDb));

        builder
            .Property(c => c.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => CategoryName.Create(fromDb));

        builder
            .Property(c => c.OwnedVehiclesCount)
            .HasColumnName("vehicles_count")
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => CategoryOwnedVehiclesCount.Create(fromDb));

        builder
            .Property(c => c.Rating)
            .HasColumnName("rating")
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => CategoryRating.Create(fromDb));

        builder.ConfigureVector();

        builder.HasIndex(c => c.Name).IsUnique().HasDatabaseName("category_unique_name_idx");
    }
}
