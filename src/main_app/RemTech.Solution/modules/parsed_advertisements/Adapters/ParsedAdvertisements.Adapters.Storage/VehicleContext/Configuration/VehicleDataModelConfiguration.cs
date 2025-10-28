using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParsedAdvertisements.Adapters.Storage.VehicleContext.DataModels;
using Shared.Infrastructure.Module.EfCore;

namespace ParsedAdvertisements.Adapters.Storage.VehicleContext.Configuration;

public sealed class VehicleDataModelConfiguration : IEntityTypeConfiguration<VehicleDataModel>
{
    public void Configure(EntityTypeBuilder<VehicleDataModel> builder)
    {
        builder.ToTable("vehicles");
        builder.HasKey(e => e.VehicleId).HasName("pk_vehicles");
        builder.Property(e => e.VehicleId).IsRequired().HasColumnName("vehicle_id");
        builder.Property(e => e.BrandId).IsRequired().HasColumnName("brand_id");
        builder.Property(e => e.CategoryId).IsRequired().HasColumnName("category_id");
        builder.Property(e => e.ModelId).IsRequired().HasColumnName("model_id");
        builder.Property(e => e.LocationId).IsRequired().HasColumnName("location_id");
        builder.Property(e => e.Price).IsRequired().HasColumnName("price");
        builder.Property(e => e.IsNds).IsRequired().HasColumnName("is_nds");
        builder.Property(e => e.Url).IsRequired().HasColumnName("url");
        builder.Property(e => e.Domain).IsRequired().HasColumnName("domain");
        builder.Property(e => e.LocationPath)
            .IsRequired()
            .HasColumnName("location_path")
            .HasColumnType("ltree");
        builder.HasIndex(e => e.LocationPath).HasMethod("gist").HasDatabaseName("idx_location_path");
        builder.Property(e => e.Photos).HasColumnName("photos").HasColumnType("jsonb").IsRequired();
        builder.ConfigureEmbeddingProperty();
        builder.HasMany(c => c.Characteristics)
            .WithOne()
            .HasForeignKey(ctx => ctx.VehicleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_vehicle_characteristics");
        builder.HasIndex(e => new { e.VehicleId, e.BrandId, e.CategoryId, e.ModelId, e.LocationId }).IsUnique();
        builder.HasIndex(e => e.Price);
    }
}