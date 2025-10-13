using Pgvector;
using RemTech.Infrastructure.PostgreSQL.Vector;
using Telemetry.Domain.TelemetryContext;

namespace Telemetry.Infrastructure.PostgreSQL.Repositories;

internal static class TelemetryRecordInfrastructureExtensions
{
    public static Vector Generate(this TelemetryRecord record, IEmbeddingGenerator generator)
    {
        string comments = string.Join(", ", record.Details.Comments.Select(c => c.Value));
        string text =
            $"{record.Details.Name} {record.Status.Value} {comments} записано: {record.OccuredAt}";
        Vector vector = new Vector(generator.Generate(text));
        return vector;
    }
}