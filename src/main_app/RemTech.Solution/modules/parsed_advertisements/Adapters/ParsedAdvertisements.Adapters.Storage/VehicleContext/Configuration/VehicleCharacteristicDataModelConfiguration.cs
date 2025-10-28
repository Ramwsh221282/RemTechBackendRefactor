using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParsedAdvertisements.Adapters.Storage.VehicleContext.DataModels;

namespace ParsedAdvertisements.Adapters.Storage.VehicleContext.Configuration;

public sealed class
    VehicleCharacteristicDataModelConfiguration : IEntityTypeConfiguration<VehicleCharacteristicDataModel>
{
    public void Configure(EntityTypeBuilder<VehicleCharacteristicDataModel> builder)
    {
        builder.ToTable("vehicle_characteristics");
        builder.HasKey(v => new { v.VehicleId, v.CharacteristicId }).HasName("pk_vehicle_characteristics");
        builder.Property(e => e.VehicleId).IsRequired().HasColumnName("vehicle_id");
        builder.Property(e => e.CharacteristicId).IsRequired().HasColumnName("characteristic_id");
    }
}