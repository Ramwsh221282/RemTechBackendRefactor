using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

public sealed class ContainedItemStatus
{
    private ContainedItemStatus(string value) => Value = value;

    public static Result<ContainedItemStatus> CreateFromString(string value)
    {
        ContainedItemStatus? status = All.FirstOrDefault(s => s.Value == value);
        return status is null ? Error.Validation($"Статус {value} невалиден.") : status;
    }

    public static readonly ContainedItemStatus PendingToSave = new("PendingToSave");
    public static readonly ContainedItemStatus Saved = new("Saved");
    private static readonly ContainedItemStatus[] All = [PendingToSave, Saved];
    public string Value { get; }
}
