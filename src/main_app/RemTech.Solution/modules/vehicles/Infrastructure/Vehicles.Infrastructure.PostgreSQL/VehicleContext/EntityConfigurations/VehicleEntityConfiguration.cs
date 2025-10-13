using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RemTech.Infrastructure.PostgreSQL;
using Vehicles.Domain.BrandContext.ValueObjects;
using Vehicles.Domain.CategoryContext.ValueObjects;
using Vehicles.Domain.LocationContext.ValueObjects;
using Vehicles.Domain.ModelContext.ValueObjects;
using Vehicles.Domain.VehicleContext;
using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.VehicleContext.EntityConfigurations;

public sealed class VehicleEntityConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles");

        builder.HasKey(v => v.Id).HasName("id");

        builder
            .Property(v => v.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Value, fromDb => new VehicleId(fromDb));

        builder
            .Property(v => v.CategoryId)
            .HasColumnName("category_id")
            .HasConversion(toDb => toDb.Value, fromDb => new CategoryId(fromDb))
            .IsRequired();

        builder
            .Property(v => v.LocationId)
            .HasColumnName("location_id")
            .HasConversion(toDb => toDb.Value, fromDb => new LocationId(fromDb))
            .IsRequired();

        builder
            .Property(v => v.BrandId)
            .HasColumnName("brand_id")
            .HasConversion(toDb => toDb.Id, fromDb => new BrandId(fromDb))
            .IsRequired();

        builder
            .Property(v => v.ModelId)
            .HasColumnName("model_id")
            .HasConversion(toDb => toDb.Value, fromDb => new VehicleModelId(fromDb))
            .IsRequired();

        builder.ComplexProperty(
            v => v.Price,
            pb =>
            {
                pb.Property(p => p.Value).HasColumnName("price_value").IsRequired();
                pb.Property(p => p.IsNds).HasColumnName("is_nds").IsRequired();
            }
        );

        builder
            .Property(v => v.Description)
            .HasColumnName("description")
            .HasConversion(toDb => toDb.Value, fromDb => new VehicleDescription(fromDb))
            .IsRequired();

        builder
            .Property(v => v.Characteristics)
            .HasColumnName("characteristics")
            .HasColumnType("jsonb")
            .HasConversion(
                toDb => toDb.VehicleCharacteristicsToJson(),
                fromDb => fromDb.JsonToVehicleCharacteristics()
            )
            .IsRequired();

        builder
            .Property(v => v.Photos)
            .HasColumnName("photos")
            .HasColumnType("jsonb")
            .IsRequired()
            .HasConversion(
                toDb => toDb.VehiclePhotosToJson(),
                fromDb => fromDb.JsonToVehiclePhotos()
            );

        builder.ConfigureVector("vehicles");

        builder.HasIndex(["Id", "CategoryId", "LocationId", "BrandId", "ModelId"]).IsUnique();

        builder
            .HasOne(v => v.Brand)
            .WithMany()
            .HasForeignKey(v => v.BrandId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(v => v.Model)
            .WithMany()
            .HasForeignKey(v => v.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(v => v.Category)
            .WithMany()
            .HasForeignKey(v => v.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(v => v.Location)
            .WithMany()
            .HasForeignKey(v => v.LocationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public static class InfrastructureVehicleExtensions
{
    public static string VehiclePhotosToJson(this VehiclePhotosCollection photos)
    {
        return JsonSerializer.Serialize(photos, JsonSerializerOptions.Default);
    }

    public static VehiclePhotosCollection JsonToVehiclePhotos(this string json)
    {
        return JsonSerializer.Deserialize<VehiclePhotosCollection>(
            json,
            JsonSerializerOptions.Default
        )!;
    }

    public static string VehicleCharacteristicsToJson(
        this VehicleCharacteristicsCollection characteristics
    )
    {
        return JsonSerializer.Serialize(characteristics, JsonSerializerOptions.Default);
    }

    public static VehicleCharacteristicsCollection JsonToVehicleCharacteristics(
        this string characteristics
    )
    {
        return JsonSerializer.Deserialize<VehicleCharacteristicsCollection>(
            characteristics,
            JsonSerializerOptions.Default
        )!;
    }
}
