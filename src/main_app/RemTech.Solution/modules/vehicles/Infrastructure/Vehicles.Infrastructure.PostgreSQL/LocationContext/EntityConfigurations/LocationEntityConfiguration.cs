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
            .HasColumnName("location_id")
            .HasConversion(toDb => toDb.Value, fromDb => LocationId.Create(fromDb));

        builder
            .Property(l => l.Rating)
            .HasColumnName("location_rating")
            .HasConversion(toDb => toDb.Value, fromDb => LocationRating.Create(fromDb))
            .IsRequired();

        builder
            .Property(l => l.VehicleCount)
            .HasColumnName("vehicles_count")
            .HasConversion(toDb => toDb.Value, fromDb => LocationVehiclesCount.Create(fromDb))
            .IsRequired();

        builder
            .Property(l => l.Address)
            .HasColumnName("address")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.ConfigureVector();
    }
}
