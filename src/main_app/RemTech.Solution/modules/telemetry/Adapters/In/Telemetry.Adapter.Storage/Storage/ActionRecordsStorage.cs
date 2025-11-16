using Shared.Infrastructure.Module.Postgres.Embeddings;
using Telemetry.Domain.Ports.Storage;

namespace Telemetry.Adapter.Storage.Storage;

public sealed partial class ActionRecordsStorage(
    IEmbeddingGenerator generator,
    ActionRecordDbContext context
) : IActionRecordsStorage;
