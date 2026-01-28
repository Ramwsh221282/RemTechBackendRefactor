using StackExchange.Redis;
using Telemetry.Core.ActionRecords;

namespace Telemetry.Infrastructure;

/// <summary>
/// Транзакция с записями действий телеметрии.
/// </summary>
/// <param name="Transaction">Транзакция Redis.</param>
/// <param name="Records">Список записей действий.</param>
public sealed record TelemetryActionRecordsTransaction(ITransaction Transaction, IReadOnlyList<ActionRecord> Records);
