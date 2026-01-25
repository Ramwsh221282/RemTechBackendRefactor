using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public sealed record SpareOem
{
	private SpareOem(string value) => Value = value;

	public string Value { get; }

	public static Result<SpareOem> Create(string value) =>
		string.IsNullOrWhiteSpace(value)
			? Error.Validation("OEM-номер запчасти не может быть пустым.")
			: Result.Success(new SpareOem(value));
}
