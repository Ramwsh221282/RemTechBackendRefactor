using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.Module.EfCore;
using Telemetry.Adapter.Storage.DataModels;
using Telemetry.Domain.Models.ValueObjects;

namespace Telemetry.Adapter.Storage.Configurations;

public sealed class TelemetryRecordConfiguration : IEntityTypeConfiguration<ActionRecordDataModel>
{
    public void Configure(EntityTypeBuilder<ActionRecordDataModel> builder)
    {
        builder.ToTable("records");
        builder.HasKey(r => r.Id).HasName("pk_record_id");
        builder.Property(r => r.Id).HasColumnName("id");
        builder.Property(r => r.InvokerId).HasColumnName("invoker_id").IsRequired();
        builder.HasIndex(r => r.InvokerId);

        builder
            .Property(r => r.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasMaxLength(ActionStatus.MaxLength);

        builder
            .Property(r => r.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(ActionName.MaxLength);

        builder.Property(r => r.OccuredAt).HasColumnName("occured_at").IsRequired();
        builder.HasIndex(r => r.OccuredAt).IsDescending();

        builder
            .Property(r => r.Comments)
            .HasColumnType("jsonb")
            .HasColumnName("comments")
            .IsRequired()
            .HasConversion(
                toDb => JsonSerializer.Serialize(toDb, JsonSerializerOptions.Default),
                fromDb =>
                    JsonSerializer.Deserialize<IEnumerable<string>>(
                        fromDb,
                        JsonSerializerOptions.Default
                    )!
            );

        builder.ConfigureEmbeddingProperty();
    }
}
