using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Models;

public readonly record struct ModelId
{
	public ModelId()
	{
		Value = Guid.NewGuid();
	}

	private ModelId(Guid value)
	{
		Value = value;
	}

	public Guid Value { get; }

	public static Result<ModelId> Create(Guid value)
	{
		return value == Guid.Empty
			? Error.Validation("Идентификатор модели не может быть пустым.")
			: new ModelId(value);
	}
}
