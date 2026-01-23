using RemTech.SharedKernel.Core.Enumerables;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public sealed class SparePhotoCollection
{
	public IReadOnlyList<SparePhoto> Value { get; }

	private SparePhotoCollection(IReadOnlyList<SparePhoto> value) => Value = value;

	public static Result<SparePhotoCollection> Create(IReadOnlyList<SparePhoto> value) =>
		value.HasDuplicates(out _) switch
		{
			true => Error.Validation("Фото запчасти не может содержать дубликаты: "),
			false => new SparePhotoCollection(value),
		};

	public static Result<SparePhotoCollection> Create(IEnumerable<string> values)
	{
		List<Result<SparePhoto>> photos = [.. values.Select(SparePhoto.Create)];
		int failuresCount = photos.Count(p => p.IsFailure);
		return failuresCount switch
		{
			0 => Create(photos.ConvertAll(p => p.Value)),
			_ => Error.Validation($"Фото запчастей содержат {failuresCount} ошибок."),
		};
	}
}
