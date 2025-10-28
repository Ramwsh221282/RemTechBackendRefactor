using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParsedAdvertisements.Adapters.Storage.ModelContext.DataModels;
using ParsedAdvertisements.Adapters.Storage.VehicleContext.DataModels;
using ParsedAdvertisements.Domain.ModelContext.ValueObjects;
using Shared.Infrastructure.Module.EfCore;

namespace ParsedAdvertisements.Adapters.Storage.ModelContext.Configuration;

public sealed class ModelDataModelConfiguration : IEntityTypeConfiguration<ModelDataModel>
{
    public void Configure(EntityTypeBuilder<ModelDataModel> builder)
    {
        builder.ToTable("models");
        builder.HasKey(x => x.Id).HasName("pk_models");
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(ModelName.MaxLength);
        builder
            .HasMany<VehicleDataModel>()
            .WithOne()
            .HasForeignKey(ad => ad.ModelId).IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_vehicle_models");
        builder.ConfigureEmbeddingProperty();
    }
}