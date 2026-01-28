using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Models;

/// <summary>
/// Идентификатор модели транспортного средства.
/// </summary>
public readonly record struct ModelId
{
	/// <summary>
	/// Создаёт новый идентификатор модели транспортного средства.
	/// </summary>
	public ModelId()
	{
		Value = Guid.NewGuid();
	}

	private ModelId(Guid value)
	{
		Value = value;
	}

	/// <summary>
	/// Уникальный идентификатор модели транспортного средства.
	/// </summary>
	public Guid Value { get; }

	/// <summary>
	/// Создаёт идентификатор модели.
	/// </summary>
	/// <param name="value">Уникальный идентификатор модели.</param>
	/// <returns>Результат создания идентификатора модели.</returns>
	public static Result<ModelId> Create(Guid value)
	{
		return value == Guid.Empty
			? Error.Validation("Идентификатор модели не может быть пустым.")
			: new ModelId(value);
	}
}
