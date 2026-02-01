using Identity.Infrastructure.Permissions.GetPermissions;

namespace WebHostApplication.Queries.Responses;

public sealed class ActionRecordsPageResponse
{
	public required GetActionRecordQueryResponse Records { get; set; }
	public required IReadOnlyList<ActionRecordsStatisticsResponse> Statistics { get; set; }
	public required IReadOnlyList<PermissionResponse>? Permissions { get; set; }
	public required IReadOnlyList<ActionRecordStatusResponse>? Statuses { get; set; }
}
