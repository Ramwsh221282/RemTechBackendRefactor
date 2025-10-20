using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using Telemetry.Domain.Models;
using Telemetry.Domain.Models.ValueObjects;
using Telemetry.Domain.Ports.Storage;

namespace Telemetry.Adapter.Storage.Storage;

public sealed partial class ActionRecordsStorage(
    IEmbeddingGenerator generator,
    ActionRecordDbContext context
) : IActionRecordsStorage;
