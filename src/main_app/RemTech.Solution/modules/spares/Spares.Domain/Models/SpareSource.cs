using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public sealed record SpareSource
{
	private SpareSource(string url) => Url = url;

	public string Url { get; }

	public static Result<SpareSource> Create(string url) =>
		string.IsNullOrWhiteSpace(url) ? Error.Validation("Ссылка на источник пустая.") : new SpareSource(url);
}
