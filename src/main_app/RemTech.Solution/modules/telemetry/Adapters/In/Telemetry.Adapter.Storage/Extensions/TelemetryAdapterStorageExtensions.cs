using Pgvector;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using Telemetry.Adapter.Storage.DataModels;
using Telemetry.Domain.Models;

namespace Telemetry.Adapter.Storage.Extensions;

public static class TelemetryAdapterStorageExtensions
{
    public static ActionRecordDataModel ConvertToDataModel(this ActionRecord record)
    {
        Guid id = record.Id.Value;
        Guid invokerId = record.InvokerId.Value;
        string name = record.Name.Value;
        string status = record.Status.Value;
        DateTime occuredAt = record.OccuredAt.OccuredAt;
        IEnumerable<string> comments = record.Comments.Select(c => c.Value);
        return new ActionRecordDataModel
        {
            Id = id,
            InvokerId = invokerId,
            Name = name,
            Status = status,
            OccuredAt = occuredAt,
            Comments = comments,
        };
    }

    public static Vector GenerateEmbedding(
        this ActionRecordDataModel record,
        IEmbeddingGenerator generator
    )
    {
        string comments = string.Join(", ", record.Comments.Select(c => c));
        string text = $"{record.Name} {record.Status} {comments} записано: {record.OccuredAt:d}";
        ReadOnlyMemory<float> embeddings = generator.Generate(text);
        return new Vector(embeddings);
    }
}
