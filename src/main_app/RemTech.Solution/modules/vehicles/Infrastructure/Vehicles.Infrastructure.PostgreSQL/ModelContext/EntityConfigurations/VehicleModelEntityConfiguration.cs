using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RemTech.Infrastructure.PostgreSQL;
using Vehicles.Domain.ModelContext;
using Vehicles.Domain.ModelContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.ModelContext.EntityConfigurations;

public sealed class VehicleModelEntityConfiguration : IEntityTypeConfiguration<VehicleModel>
{
    public void Configure(EntityTypeBuilder<VehicleModel> builder)
    {
        builder.ToTable("models");

        builder.HasKey(m => m.Id).HasName("pk_models");

        builder
            .Property(m => m.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Value, fromDb => new VehicleModelId(fromDb));

        builder
            .Property(m => m.Name)
            .HasColumnName("name")
            .HasConversion(toDb => toDb.Value, fromDb => new VehicleModelName(fromDb))
            .IsRequired();

        builder
            .Property(m => m.VehiclesCount)
            .HasColumnName("vehicles_count")
            .HasConversion(toDb => toDb.Value, fromDb => new VehicleModelVehicleCount(fromDb))
            .IsRequired();

        builder
            .Property(m => m.Rating)
            .HasColumnName("rating")
            .HasConversion(toDb => toDb.Value, fromDb => new VehicleModelRating(fromDb))
            .IsRequired();

        builder.HasIndex(m => m.Name).IsUnique().HasDatabaseName("unique_model_name_idx");

        builder.ConfigureVector("models");
    }
}
