using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Oems;

/// <summary>
/// Идентификатор OEM-номера запчасти.
/// </summary>
public readonly record struct SpareOemId
{
	public Guid Value { get; }

	public SpareOemId()
	{
		Value = Guid.NewGuid();
	}

	private SpareOemId(Guid id)
	{
		Value = id;
	}

	/// <summary>
	/// Создает идентификатор OEM-номера запчасти из GUID.
	/// </summary>
	public static Result<SpareOemId> FromGuid(Guid id)
	{
		return id == Guid.Empty ? Error.Validation("SpareOemId не может быть пустым GUID.") : new SpareOemId(id);
	}

	/// <summary>
	/// Создает новый идентификатор OEM-номера запчасти.
	/// </summary>
	public static SpareOemId New()
	{
		return new SpareOemId();
	}
}
