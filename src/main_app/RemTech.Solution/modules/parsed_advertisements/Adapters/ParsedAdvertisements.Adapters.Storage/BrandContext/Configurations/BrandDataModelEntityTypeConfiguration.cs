using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParsedAdvertisements.Adapters.Storage.BrandContext.DataModels;
using ParsedAdvertisements.Adapters.Storage.VehicleContext.DataModels;
using ParsedAdvertisements.Domain.BrandContext.ValueObjects;
using Shared.Infrastructure.Module.EfCore;

namespace ParsedAdvertisements.Adapters.Storage.BrandContext.Configurations;

public sealed class BrandDataModelEntityTypeConfiguration : IEntityTypeConfiguration<BrandDataModel>
{
    public void Configure(EntityTypeBuilder<BrandDataModel> builder)
    {
        builder.ToTable("brands");
        builder.HasKey(x => x.Id).HasName("pk_brands");
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(BrandName.Length);
        builder
            .HasMany<VehicleDataModel>()
            .WithOne()
            .HasForeignKey(ctx => ctx.BrandId).IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_vehicle_brands");
        builder.ConfigureEmbeddingProperty();
    }
}