using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public sealed record SparePhoto
{
	private SparePhoto(string value) => Value = value;

	public string Value { get; }

	public static Result<SparePhoto> Create(string value) =>
		string.IsNullOrWhiteSpace(value)
			? Error.Validation("Фото запчасти не может быть пустым.")
			: Result.Success(new SparePhoto(value));
}
