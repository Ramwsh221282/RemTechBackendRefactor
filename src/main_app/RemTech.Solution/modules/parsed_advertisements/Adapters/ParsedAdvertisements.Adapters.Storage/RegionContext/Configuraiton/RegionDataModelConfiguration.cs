using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParsedAdvertisements.Adapters.Storage.RegionContext.DataModels;
using ParsedAdvertisements.Adapters.Storage.VehicleContext.DataModels;
using ParsedAdvertisements.Domain.RegionContext.ValueObjects;
using Shared.Infrastructure.Module.EfCore;

namespace ParsedAdvertisements.Adapters.Storage.RegionContext.Configuraiton;

public class RegionDataModelConfiguration : IEntityTypeConfiguration<RegionDataModel>
{
    public void Configure(EntityTypeBuilder<RegionDataModel> builder)
    {
        builder.ToTable("regions");
        builder.HasKey(x => x.Id).HasName("pk_regions");
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();

        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(RegionName.MaxLength);

        builder.Property(x => x.Kind)
            .HasColumnName("kind")
            .IsRequired()
            .HasMaxLength(RegionKind.MaxLength);

        builder
            .HasMany<VehicleDataModel>()
            .WithOne()
            .HasForeignKey(ad => ad.LocationId)
            .IsRequired()
            .HasConstraintName("fk_vehicle_locations")
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureEmbeddingProperty("regions");
    }
}