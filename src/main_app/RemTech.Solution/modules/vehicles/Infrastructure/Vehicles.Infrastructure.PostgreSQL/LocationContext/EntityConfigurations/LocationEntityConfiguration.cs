using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RemTech.Infrastructure.PostgreSQL;
using Vehicles.Domain.LocationContext;
using Vehicles.Domain.LocationContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.LocationContext.EntityConfigurations;

public sealed class LocationEntityConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.Id).HasName("pk_locations");

        builder
            .Property(l => l.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Value, fromDb => new LocationId(fromDb));

        builder
            .Property(l => l.Rating)
            .HasColumnName("rating")
            .HasConversion(toDb => toDb.Value, fromDb => new LocationRating(fromDb))
            .IsRequired();

        builder
            .Property(l => l.VehicleCount)
            .HasColumnName("vehicles_count")
            .HasConversion(toDb => toDb.Value, fromDb => new LocationVehiclesCount(fromDb))
            .IsRequired();

        builder
            .Property(l => l.Address)
            .HasColumnName("address")
            .HasColumnType("jsonb")
            .HasConversion(toDb => toDb.LocationToJson(), fromDb => fromDb.JsonToLocation())
            .IsRequired();

        builder.ConfigureVector("locations");
    }
}
