using Telemetry.Domain.Models.ValueObjects;

namespace Telemetry.Domain.Ports.Storage;

public sealed record ReadActions(
    int Page,
    int PageSize,
    ActionInvokerId? InvokerId = null,
    ActionStatus? Status = null,
    ActionName? Name = null,
    ActionDate? OccuredAtMin = null,
    ActionDate? OccuredAtMax = null,
    string? TextSearch = null,
    IEnumerable<string>? OrderNames = null,
    CancellationToken ct = default
)
{
    public string OrderMode { get; } = "ASC";

    public ReadActions(
        int Page,
        int PageSize,
        ActionInvokerId? InvokerId = null,
        ActionStatus? Status = null,
        ActionName? Name = null,
        ActionDate? OccuredAtMin = null,
        ActionDate? OccuredAtMax = null,
        string? TextSearch = null,
        IEnumerable<string>? OrderNames = null,
        string? orderMode = null,
        CancellationToken ct = default
    )
        : this(
            Page,
            PageSize,
            InvokerId,
            Status,
            Name,
            OccuredAtMin,
            OccuredAtMax,
            TextSearch,
            OrderNames,
            ct
        )
    {
        if (!string.IsNullOrWhiteSpace(orderMode) && orderMode == "DESC")
            OrderMode = orderMode;
    }
}
