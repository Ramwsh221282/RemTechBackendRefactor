using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

public sealed class ContainedItemStatus
{
	public static readonly ContainedItemStatus PendingToSave = new("PendingToSave");
	public static readonly ContainedItemStatus Saved = new("Saved");
	private static readonly ContainedItemStatus[] _all = [PendingToSave, Saved];

	private ContainedItemStatus(string value) => Value = value;

	public string Value { get; }

	public static Result<ContainedItemStatus> CreateFromString(string value)
	{
		ContainedItemStatus? status = _all.FirstOrDefault(s => s.Value == value);
		return status is null ? Error.Validation($"Статус {value} невалиден.") : status;
	}
}
