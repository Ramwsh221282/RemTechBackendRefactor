using Telemetry.Domain.Models;

namespace Telemetry.Domain.Ports.Storage;

public sealed record ActionRecordsCollection(long TotalCount, IEnumerable<ActionRecord> Records);
