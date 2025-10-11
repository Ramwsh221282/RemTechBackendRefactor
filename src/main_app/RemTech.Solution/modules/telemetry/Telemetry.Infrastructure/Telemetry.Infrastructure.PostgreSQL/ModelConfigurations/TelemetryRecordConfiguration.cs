using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;
using RemTech.Core.Shared.Result;
using Telemetry.Domain.TelemetryContext;
using Telemetry.Domain.TelemetryContext.ValueObjects;

namespace Telemetry.Infrastructure.PostgreSQL.ModelConfigurations;

public sealed class TelemetryRecordConfiguration : IEntityTypeConfiguration<TelemetryRecord>
{
    public void Configure(EntityTypeBuilder<TelemetryRecord> builder)
    {
        builder.ToTable("records");
        builder.HasKey(r => r.RecordId).HasName("pk_record_id");

        builder
            .Property(r => r.RecordId)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Value, fromDb => TelemetryRecordId.Create(fromDb));

        builder
            .Property(r => r.InvokerId)
            .HasColumnName("invoker_id")
            .HasConversion(toDb => toDb.Value, fromDb => TelemetryInvokerId.Create(fromDb))
            .IsRequired();
        builder.HasIndex(r => r.InvokerId);

        builder
            .Property(r => r.Status)
            .HasColumnName("status")
            .HasConversion(toDb => toDb.Value, fromDb => TelemetryActionStatus.Create(fromDb))
            .IsRequired();

        builder
            .Property(r => r.OccuredAt)
            .HasColumnName("occured_at")
            .IsRequired()
            .HasConversion(toDb => toDb.OccuredAt, fromDb => TelemetryRecordDate.Create(fromDb));

        builder.HasIndex(r => r.OccuredAt).IsDescending();

        builder
            .Property(r => r.Details)
            .HasColumnType("jsonb")
            .HasColumnName("details")
            .IsRequired()
            .HasConversion(
                toDb => TelemetryDetailsAsJson(toDb),
                fromDb => JsonAsTelemetryDetailsAction(fromDb)
            );

        builder.Property<Vector>("embedding").HasColumnType("vector(1024)").IsRequired(false);
        builder
            .HasIndex("embedding")
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops")
            .HasDatabaseName("idx_hnsw_record");
    }

    private static string TelemetryDetailsAsJson(TelemetryActionDetails details)
    {
        string json = JsonSerializer.Serialize(details, JsonSerializerOptions.Default);
        return json;
    }

    private static TelemetryActionDetails JsonAsTelemetryDetailsAction(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;
        string? name = root.GetProperty("Name").GetProperty("Value").GetString();
        Status<TelemetryActionName> actionName = TelemetryActionName.Create(name);
        if (actionName.IsFailure)
            throw new InvalidOperationException(
                $"Invalid mapping jsonb to {nameof(TelemetryActionName)}"
            );

        List<TelemetryActionComment> comments = [];
        foreach (JsonElement entry in root.GetProperty("Comments").EnumerateArray())
        {
            string? value = entry.GetProperty("Value").GetString();
            Status<TelemetryActionComment> comment = TelemetryActionComment.Create(value);
            if (comment.IsFailure)
                throw new InvalidOperationException(
                    $"Invalid mapping jsonb to {nameof(TelemetryActionName)}"
                );
            comments.Add(comment);
        }

        return new TelemetryActionDetails(actionName, comments);
    }
}
