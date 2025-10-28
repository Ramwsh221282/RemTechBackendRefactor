using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParsedAdvertisements.Adapters.Storage.CategoryContext.Configurations;
using ParsedAdvertisements.Adapters.Storage.VehicleContext.DataModels;
using ParsedAdvertisements.Domain.CategoryContext.ValueObjects;
using Shared.Infrastructure.Module.EfCore;

namespace ParsedAdvertisements.Adapters.Storage.CategoryContext.DataModels;

public sealed class CategoryDataModelEntityTypeConfiguration : IEntityTypeConfiguration<CategoryDataModel>
{
    public void Configure(EntityTypeBuilder<CategoryDataModel> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(x => x.Id).HasName("pk_categories");
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(CategoryName.MaxLength);
        builder
            .HasMany<VehicleDataModel>()
            .WithOne()
            .HasForeignKey(ad => ad.CategoryId).IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_vehicle_categories");
        builder.ConfigureEmbeddingProperty();
    }
}