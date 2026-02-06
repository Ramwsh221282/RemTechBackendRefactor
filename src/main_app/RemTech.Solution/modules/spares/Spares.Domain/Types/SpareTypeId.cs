using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Types;

/// <summary>
/// Идентификатор типа запчасти.
/// </summary>
public readonly record struct SpareTypeId
{
	public Guid Value { get; }

	public SpareTypeId()
	{
		Value = Guid.NewGuid();
	}

	private SpareTypeId(Guid id)
	{
		Value = id;
	}

	/// <summary>
	/// Создает идентификатор типа запчасти из GUID.
	/// </summary>
	public static Result<SpareTypeId> FromGuid(Guid id)
	{
		return id == Guid.Empty ? Error.Validation("SpareTypeId не может быть пустым GUID.") : new SpareTypeId(id);
	}

	/// <summary>
	/// Создает новый идентификатор типа запчасти.
	/// </summary>
	public static SpareTypeId New()
	{
		return new SpareTypeId();
	}
}
